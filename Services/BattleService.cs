using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Enums;

namespace ProyectoGalactico.Services;

public class BattleService(AppDbContext db, IEncryptionService encryption)
{
    private const double SupportRate = 0.5;
    private const double MinFactor = 0.9;
    private const double MaxFactor = 1.1;
    private readonly AppDbContext _db = db;
    private readonly IEncryptionService _encryption = encryption;

    public async Task<ServiceResult<BattleResult>> SimulateAsync(int eventId, int? seed)
    {
        var eventEntity = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        if (eventEntity is null)
            return ServiceResult<BattleResult>.NotFound($"No existe el evento {eventId}.");

        var characters = await _db.Characters.AsNoTracking()
            .Include(c => c.Cards)
            .Where(c => eventEntity.ParticipantsIds.Contains(c.Id))
            .ToListAsync();

        var autobots = new List<FighterScore>();
        var decepticons = new List<FighterScore>();
        var neutrals = new List<FighterScore>();
        var missingCards = new List<string>();

        foreach (var character in characters)
        {
            var card = character.Cards.OrderByDescending(c => c.DangerousLevel)
                .ThenBy(c => c.Id).FirstOrDefault();
            if (card is null)
            {
                missingCards.Add(character.Name);
                continue;
            }

            var fighter = new FighterScore(character.Id, character.Name,
                card.Id, card.DangerousLevel, PowerCalculator.Bonus(character),
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
            return ServiceResult<BattleResult>.Invalid($"Faltan cartas para: {string.Join(", ", missingCards)}.");
        if (autobots.Count == 0 || decepticons.Count == 0)
            return ServiceResult<BattleResult>.Invalid(
                "La simulación necesita al menos un participante Autobot y uno Decepticon. Los neutrales solo dan apoyo.");

        var autobotBase = autobots.Sum(f => f.Score);
        var decepticonBase = decepticons.Sum(f => f.Score);

        var supportPool = neutrals.Sum(f => f.Score) * SupportRate;

        var (autobotSupport, decepticonSupport, supportNote) =
            DistributeSupport(autobotBase, decepticonBase, supportPool);

        var usedSeed = seed ?? Random.Shared.Next();
        var random = new Random(usedSeed);

        var autobotFactor = RollFactor(random);
        var decepticonFactor = RollFactor(random);

        var autobotFinal = Math.Round((autobotBase + autobotSupport) * autobotFactor, 2);
        var decepticonFinal = Math.Round((decepticonBase + decepticonSupport) * decepticonFactor, 2);

        var winner = autobotFinal > decepticonFinal ? "Autobot"
            : decepticonFinal > autobotFinal ? "Decepticon"
            : "Empate";

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
            usedSeed);

        return ServiceResult<BattleResult>.Ok(result);
    }

    private static (double autobot, double decepticon, string note) DistributeSupport(
        double autobotBase, double decepticonBase, double support)
    {
        if (support == 0)
            return (0, 0, "No hubo neutrales: sin apoyo.");

        if (autobotBase < decepticonBase)
            return (support, 0, "Los neutrales apoyaron al bando Autobot (el más débil).");

        if (decepticonBase < autobotBase)
            return (0, support, "Los neutrales apoyaron al bando Decepticon (el más débil).");

        return (support / 2, support / 2,
            "Bandos empatados: el apoyo neutral se repartió por igual.");
    }

    private static double RollFactor(Random random) =>
        Math.Round(MinFactor + random.NextDouble() * (MaxFactor - MinFactor), 3);
}
