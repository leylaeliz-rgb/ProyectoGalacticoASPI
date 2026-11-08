using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos.Responses;

namespace ProyectoGalactico.Services
{
    public class RankingService(CardService cards)
    {
        public ServiceResult<List<RankingItem>> GetRanking(string? por, string? faccion)
        {
            var criterion = (por ?? "poder").Trim().ToLowerInvariant();
            if (criterion != "poder")
                return ServiceResult<List<RankingItem>>.Invalid("Criterio no válido. Por ahora solo se admite por=poder.");

            char? code = null;
            if (faccion is not null)
            {
                code = GameCodes.ParseFaction(faccion);
                if (code is null)
                    return ServiceResult<List<RankingItem>>.Invalid("Facción no válida. Usa Rebelde, Imperio o Neutral.");
            }

            lock (DataForModels.Sync)
            {
                var ranking = DataForModels.Characters
                    .Where(c => code is null || c.faccion == code)
                    .Select(c => (Person: c, Best: cards.GetBestCard(c.Id)))
                    .Where(x => x.Best is not null)
                    .Select(x => (x.Person, Best: x.Best!, Score: PowerCalculator.Score(x.Person, x.Best!)))
                    .OrderByDescending(x => x.Score).ThenBy(x => x.Person.Id) // empate: menor id primero
                    .Select((x, i) => new RankingItem(i + 1, x.Person.Id, x.Person.Name,
                        GameCodes.FactionName(x.Person.faccion), x.Person.afiliacion, x.Best.Id,
                        x.Best.dangerousLevel, PowerCalculator.Bonus(x.Person), x.Score))
                    .ToList();

                return ServiceResult<List<RankingItem>>.Ok(ranking);
            }
        }
    }
}