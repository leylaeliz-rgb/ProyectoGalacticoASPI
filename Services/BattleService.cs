
using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Enums;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Services;

public class BattleService(AppDbContext db, IEncryptionService encryption)
{
    private const double SupportRate = 0.5;
    private const double MinFactor = 0.9;
    private const double MaxFactor = 1.1;
    private const int ReappearanceChance = 50;

    private readonly AppDbContext _db = db;
    private readonly IEncryptionService _encryption = encryption;

    public async Task<ServiceResult<BattleResult>> SimulateAsync(int eventId, int? seed)
    {
        var eventEntity = await _db.Events
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity is null)
            return ServiceResult<BattleResult>.NotFound(
                $"No existe el evento {eventId}.");

        if (!string.IsNullOrWhiteSpace(eventEntity.Winner))
            return ServiceResult<BattleResult>.Invalid(
                "Este evento ya fue simulado.");

        var characters = await _db.Characters
            .Include(c => c.Cards)
            .Where(c => eventEntity.ParticipantsIds.Contains(c.Id))
            .OrderBy(c => c.Id)
            .ToListAsync();

        var usedSeed = seed ?? Random.Shared.Next();
        var random = new Random(usedSeed);

        var autobots = new List<FighterScore>();
        var decepticons = new List<FighterScore>();
        var neutrals = new List<FighterScore>();
        var missingCards = new List<string>();
        var consequences = new List<BattleConsequence>();

        // 1. Comprobar estados y determinar reapariciones.
        var appearedCharacters = new List<(Character Character, bool Reappeared)>();
        var selectedCards = new Dictionary<int, CardCharacter>();

        foreach (var character in characters)
        {
            if (character.CharacterState == CharacterState.Dead)
            {
                return ServiceResult<BattleResult>.Invalid(
                    $"El personaje {character.Name} está muerto y no puede participar.");
            }

            if (character.CharacterState == CharacterState.Unknown)
            {
                int reappearanceRoll = random.Next(1, 101);

                if (reappearanceRoll > ReappearanceChance)
                {
                    consequences.Add(new BattleConsequence(
                        character.Id,
                        character.Name,
                        null,
                        reappearanceRoll,
                        null,
                        BattleOutcome.DidNotAppear,
                        false));

                    continue;
                }

                character.CharacterState = CharacterState.Alive;

                appearedCharacters.Add((character, true));
            }
            else
            {
                appearedCharacters.Add((character, false));
            }
        }

        // 2. Seleccionar la mejor carta disponible de cada participante.
        foreach (var (character, reappeared) in appearedCharacters)
        {
            var card = character.Cards
                .Where(c => c.IsAvailable)
                .OrderByDescending(c => c.DangerousLevel)
                .ThenBy(c => c.Id)
                .FirstOrDefault();

            if (card is null)
            {
                missingCards.Add(character.Name);
                continue;
            }
            selectedCards[character.Id] = card;

            var fighter = new FighterScore(
                character.Id,
                character.Name,
                card.Id,
                card.DangerousLevel,
                PowerCalculator.Bonus(character),
                PowerCalculator.Score(character, card));

            switch (character.Faction)
            {
                case Faction.Autobot:
                    autobots.Add(fighter);
                    break;

                case Faction.Decepticon:
                    decepticons.Add(fighter);
                    break;

                default:
                    neutrals.Add(fighter);
                    break;
            }
        }

        if (missingCards.Count > 0)
            return ServiceResult<BattleResult>.Invalid(
                $"Faltan cartas disponibles para: {string.Join(", ", missingCards)}.");

        if (autobots.Count == 0 || decepticons.Count == 0)
            return ServiceResult<BattleResult>.Invalid(
                "La simulación necesita al menos un participante Autobot y uno Decepticon. Los neutrales solo dan apoyo.");

        // 3. Calcular poder y apoyo neutral.
        var autobotBase = autobots.Sum(f => f.Score);
        var decepticonBase = decepticons.Sum(f => f.Score);

        var supportPool = neutrals.Sum(f => f.Score) * SupportRate;

        var (autobotSupport, decepticonSupport, supportNote) =
            DistributeSupport(autobotBase, decepticonBase, supportPool);

        // 4. Calcular factores aleatorios.
        var autobotFactor = RollFactor(random);
        var decepticonFactor = RollFactor(random);

        var autobotFinal = Math.Round(
            (autobotBase + autobotSupport) * autobotFactor, 2);

        var decepticonFinal = Math.Round(
            (decepticonBase + decepticonSupport) * decepticonFactor, 2);

        var winner = autobotFinal > decepticonFinal ? "Autobot"
            : decepticonFinal > autobotFinal ? "Decepticon"
            : "Empate";

        // 5. Determinar qué bando recibe las consecuencias.
        List<FighterScore> injuredFighters;

        if (winner == "Autobot")
            injuredFighters = decepticons;
        else if (winner == "Decepticon")
            injuredFighters = autobots;
        else
            injuredFighters = new List<FighterScore>();

        var injuredIds = injuredFighters
            .Select(f => f.CharacterId)
            .ToHashSet();

        // 6. Resolver consecuencias de los participantes que aparecieron.
        foreach (var (character, reappeared) in appearedCharacters)
        {
            var card = selectedCards[character.Id];

            var outcome = BattleOutcome.Unharmed;
            int? roll = null;

            if (injuredIds.Contains(character.Id))
            {
                roll = random.Next(1, 101);

                if (roll < card.SurvivalRatio)
                {
                    outcome = BattleOutcome.Survived;
                    card.IsAvailable = false;
                    character.CharacterState = CharacterState.Alive;
                }
                else if (roll > card.SurvivalRatio)
                {
                    outcome = BattleOutcome.Died;
                    card.IsAvailable = false;
                    character.CharacterState = CharacterState.Dead;

                    eventEntity.DeadIds = eventEntity.DeadIds
                        .Append(character.Id)
                        .Distinct()
                        .ToList();
                }
                else
                {
                    outcome = BattleOutcome.Disappeared;
                    character.CharacterState = CharacterState.Unknown;
                }
            }

            var consequence = new BattleConsequence(
                character.Id,
                character.Name,
                card.Id,
                roll,
                card.SurvivalRatio,
                outcome,
                reappeared);

            consequences.Add(consequence);

            _db.BattleRecords.Add(new BattleRecord
            {
                EventId = eventEntity.Id,
                CharacterId = character.Id,
                CardId = card.Id,
                Outcome = outcome,
                Roll = roll,
                SurvivalRatio = card.SurvivalRatio,
                Reappeared = reappeared
            });
        }

        // 7. Registrar los que no aparecieron.
        foreach (var consequence in consequences.Where(
            c => c.Outcome == BattleOutcome.DidNotAppear))
        {
            _db.BattleRecords.Add(new BattleRecord
            {
                EventId = eventEntity.Id,
                CharacterId = consequence.CharacterId,
                CardId = null,
                Outcome = BattleOutcome.DidNotAppear,
                Roll = consequence.Roll,
                SurvivalRatio = null,
                Reappeared = false
            });
        }

        // 8. Guardar ganador y todos los cambios en una sola operación.
        eventEntity.Winner = _encryption.Encrypt(winner);

        await _db.SaveChangesAsync();

        var criteria =
            $"Poder de cada participante = peligrosidad de su mejor carta + {PowerCalculator.SensitiveBonus} si es sensible a la Fuerza. " +
            $"Cada bando suma sus poderes, recibe el apoyo neutral si corresponde y se multiplica por un factor aleatorio entre {MinFactor} y {MaxFactor}. " +
            "Gana el total final más alto.";

        var result = new BattleResult(
            eventEntity.Id,
            eventEntity.Name,
            new SideResult("Autobot", autobots, autobotBase,
                Math.Round(autobotSupport, 2), autobotFactor, autobotFinal),
            new SideResult("Decepticon", decepticons, decepticonBase,
                Math.Round(decepticonSupport, 2), decepticonFactor, decepticonFinal),
            neutrals,
            supportNote,
            winner,
            criteria,
            usedSeed,
            consequences);

        return ServiceResult<BattleResult>.Ok(result);
    }

    private static (double autobot, double decepticon, string note)
        DistributeSupport(double autobotBase, double decepticonBase, double support)
    {
        if (support == 0)
            return (0, 0, "No hubo neutrales: sin apoyo.");

        if (autobotBase < decepticonBase)
            return (support, 0,
                "Los neutrales apoyaron al bando Autobot (el más débil).");

        if (decepticonBase < autobotBase)
            return (0, support,
                "Los neutrales apoyaron al bando Decepticon (el más débil).");

        return (support / 2, support / 2,
            "Bandos empatados: el apoyo neutral se repartió por igual.");
    }

    private static double RollFactor(Random random) =>
        Math.Round(
            MinFactor + random.NextDouble() * (MaxFactor - MinFactor), 3);
}
