using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Enums;

namespace ProyectoGalactico.Services
{
    public class BattleService(CardService cards)
    {
        private const double SupportRate = 0.5;  // los neutrales aportan la mitad de su poder
        private const double MinFactor = 0.9;
        private const double MaxFactor = 1.1;

        public ServiceResult<BattleResult> Simulate(int eventId, int? seed)
        {
            lock (DataForModels.Sync)
            {
                var index = DataForModels.Events.FindIndex(e => e.Id == eventId);
                if (index < 0) return ServiceResult<BattleResult>.NotFound($"No existe el evento {eventId}.");
                var ev = DataForModels.Events[index];

                var decepticons = new List<FighterScore>();
                var autobots = new List<FighterScore>();
                var neutrals = new List<FighterScore>();
                var missingCards = new List<string>();

                foreach (var pid in ev.participantIds)
                {
                    var character = DataForModels.Characters.FirstOrDefault(c => c.Id == pid);
                    if (character is null) continue;

                    var card = cards.GetBestCard(pid);
                    if (card is null) { missingCards.Add(character.Name); continue; }

                    var fighter = new FighterScore(character.Id, character.Name, character.afiliacion,
                        card.Id, card.dangerousLevel, PowerCalculator.Bonus(character),
                        PowerCalculator.Score(character, card));

                    switch (character.faccion)
                    {
                        case Faction.Decepticon : decepticons.Add(fighter); break;
                        case Faction.Autobot : autobots.Add(fighter); break;
                        default: neutrals.Add(fighter); break;
                    }
                }

                // Todos los errores se detectan antes de guardar nada
                if (missingCards.Count > 0)
                    return ServiceResult<BattleResult>.Invalid($"Faltan cartas para: {string.Join(", ", missingCards)}.");
                if (rebels.Count == 0 || empire.Count == 0)
                    return ServiceResult<BattleResult>.Invalid(
                        "La simulación necesita al menos un participante Rebelde y uno Imperio. Los neutrales solo dan apoyo.");

                double rebelBase = rebels.Sum(f => f.Score);
                double empireBase = empire.Sum(f => f.Score);
                double supportPool = neutrals.Sum(f => f.Score) * SupportRate;

                var (rebelSupport, empireSupport, supportNote) = DistributeSupport(rebelBase, empireBase, supportPool);

                // Siempre hay seed: si no la mandan, se genera y se devuelve para poder repetir el resultado
                var usedSeed = seed ?? Random.Shared.Next();
                var rng = new Random(usedSeed);
                var rebelFactor = RollFactor(rng);
                var empireFactor = RollFactor(rng);

                var rebelFinal = Math.Round((rebelBase + rebelSupport) * rebelFactor, 2);
                var empireFinal = Math.Round((empireBase + empireSupport) * empireFactor, 2);

                var winner = rebelFinal > empireFinal ? "Rebelde"
                           : empireFinal > rebelFinal ? "Imperio"
                           : "Empate";

                DataForModels.Events[index] = ev with { winner = winner };

                var criteria =
                    $"Poder de cada participante = peligrosidad de su mejor carta + {PowerCalculator.SensitiveBonus} si es sensible a la Fuerza. " +
                    $"Cada bando suma sus poderes, recibe el apoyo neutral si corresponde y se multiplica por un factor aleatorio entre {MinFactor} y {MaxFactor}. " +
                    "Gana el total final más alto.";

                var result = new BattleResult(ev.Id, ev.name,
                    new SideResult("Rebelde", rebels, rebelBase, Math.Round(rebelSupport, 2), rebelFactor, rebelFinal),
                    new SideResult("Imperio", empire, empireBase, Math.Round(empireSupport, 2), empireFactor, empireFinal),
                    neutrals, supportNote, winner, criteria, usedSeed);

                return ServiceResult<BattleResult>.Ok(result);
            }
        }

        // Apoyo neutral
        private static (double rebel, double empire, string note) DistributeSupport(
            double rebelBase, double empireBase, double support)
        {
            if (support == 0) return (0, 0, "No hubo neutrales: sin apoyo.");
            if (rebelBase < empireBase) return (support, 0, "Los neutrales apoyaron al bando Rebelde (el más débil).");
            if (empireBase < rebelBase) return (0, support, "Los neutrales apoyaron al bando Imperio (el más débil).");
            return (support / 2, support / 2, "Bandos empatados: el apoyo neutral se repartió por igual.");
        }

        private static double RollFactor(Random rng) =>
            Math.Round(MinFactor + rng.NextDouble() * (MaxFactor - MinFactor), 3);
    }
}