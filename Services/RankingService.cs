using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Enums;
using static ProyectoGalactico.Dtos.Responses.RankingResponses;

namespace ProyectoGalactico.Services;

public class RankingService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<ServiceResult<List<RankingItem>>> GetRankingAsync(string? by, string? faction)
    {
        if (!string.IsNullOrWhiteSpace(by) &&
            !string.Equals(by.Trim(), "poder", StringComparison.OrdinalIgnoreCase))
            return ServiceResult<List<RankingItem>>.Invalid("Criterio no válido. Por ahora solo se admite por=poder.");

        Faction? parsedFaction = null;
        if (!string.IsNullOrWhiteSpace(faction))
        {
            if (!Enum.TryParse<Faction>(faction, true, out var value) || !Enum.IsDefined(value))
                return ServiceResult<List<RankingItem>>.Invalid("Facción no válida. Usa Autobot, Decepticon o Neutral.");
            parsedFaction = value;
        }

        var query = _db.Characters.AsNoTracking().Include(c => c.Cards).AsQueryable();
        if (parsedFaction.HasValue) query = query.Where(c => c.Faction == parsedFaction.Value);

        var characters = await query.ToListAsync();
        var ranking = characters
            .Where(c => c.Cards.Count > 0)
            .Select(c =>
            {
                var bestCard = c.Cards
                    .Where(card => card.IsAvailable)
                    .OrderByDescending(card => card.DangerousLevel)
                    .ThenBy(card => card.Id)
                    .FirstOrDefault();
                return new
                {
                    Character = c,
                    Card = bestCard,
                    Score = PowerCalculator.Score(c, bestCard)
                };
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Character.Id)
            .Select((x, index) => new RankingItem(index + 1, x.Character.Id, x.Character.Name,
                (x.Character.Faction).ToString(), x.Card.Id,
                x.Card.DangerousLevel, PowerCalculator.Bonus(x.Character), x.Score))
            .ToList();

        return ServiceResult<List<RankingItem>>.Ok(ranking);
    }
}
