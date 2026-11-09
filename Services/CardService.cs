using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Services;

public class CardService(AppDbContext db) : ICardService
{
    private readonly AppDbContext _db = db;

    public async Task<List<CardCharacterDto>> GetAllAsync(int? characterId, int? minDangerousLevel)
    {
        var query = _db.CardsCharacter.AsNoTracking().AsQueryable();
        if (characterId.HasValue) query = query.Where(c => c.CharacterId == characterId.Value);
        if (minDangerousLevel.HasValue) query = query.Where(c => c.DangerousLevel >= minDangerousLevel.Value);
        var cards = await query.OrderBy(c => c.Id).ToListAsync();
        return cards.Select(ToDto).ToList();
    }

    public async Task<ServiceResult<CardCharacterDto>> GetByIdAsync(int id)
    {
        var card = await _db.CardsCharacter.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return card is null
            ? ServiceResult<CardCharacterDto>.NotFound($"No existe la carta {id}.")
            : ServiceResult<CardCharacterDto>.Ok(ToDto(card));
    }

    public async Task<CardCharacter?> GetBestCardAsync(int characterId)
    {
        return await _db.CardsCharacter.AsNoTracking()
            .Where(c => c.CharacterId == characterId)
            .OrderByDescending(c => c.DangerousLevel)
            .ThenBy(c => c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<CardCharacterDto>> CreateAsync(CardCharacterCreateDto dto)
    {
        var error = await ValidateAsync(dto);
        if (error is not null) return ServiceResult<CardCharacterDto>.Invalid(error);

        var card = new CardCharacter
        {
            CharacterId = dto.CharacterId,
            Power = dto.Power.Trim(),
            SpecialHability = dto.SpecialHability.Trim(),
            Weapon = dto.Weapon.Trim(),
            DangerousLevel = dto.DangerousLevel,
            PictureUrl = dto.PictureUrl.Trim()
        };

        _db.CardsCharacter.Add(card);
        await _db.SaveChangesAsync();
        return ServiceResult<CardCharacterDto>.Ok(ToDto(card));
    }

    public async Task<ServiceResult<CardCharacterDto>> UpdateAsync(int id, CardCharacterCreateDto dto)
    {
        var card = await _db.CardsCharacter.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null) return ServiceResult<CardCharacterDto>.NotFound($"No existe la carta {id}.");

        var error = await ValidateAsync(dto);
        if (error is not null) return ServiceResult<CardCharacterDto>.Invalid(error);

        card.CharacterId = dto.CharacterId;
        card.Power = dto.Power.Trim();
        card.SpecialHability = dto.SpecialHability.Trim();
        card.Weapon = dto.Weapon.Trim();
        card.DangerousLevel = dto.DangerousLevel;
        card.PictureUrl = dto.PictureUrl.Trim();

        await _db.SaveChangesAsync();
        return ServiceResult<CardCharacterDto>.Ok(ToDto(card));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var card = await _db.CardsCharacter.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null) return ServiceResult<bool>.NotFound($"No existe la carta {id}.");
        _db.CardsCharacter.Remove(card);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private async Task<string?> ValidateAsync(CardCharacterCreateDto dto)
    {
        if (!await _db.Characters.AnyAsync(c => c.Id == dto.CharacterId))
            return $"No existe el personaje {dto.CharacterId}.";
        if (string.IsNullOrWhiteSpace(dto.Power)) return "El poder es obligatorio.";
        if (string.IsNullOrWhiteSpace(dto.SpecialHability)) return "La habilidad especial es obligatoria.";
        if (string.IsNullOrWhiteSpace(dto.Weapon)) return "El arma es obligatoria.";
        if (dto.DangerousLevel is < 1 or > 10) return "El nivel de peligrosidad debe estar entre 1 y 10.";
        if (!Uri.TryCreate(dto.PictureUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return "La imagen debe ser una URL válida (http o https).";
        return null;
    }

    private static CardCharacterDto ToDto(CardCharacter card) => new()
    {
        Id = card.Id,
        CharacterId = card.CharacterId,
        Power = card.Power,
        SpecialHability = card.SpecialHability,
        Weapon = card.Weapon,
        DangerousLevel = card.DangerousLevel,
        PictureUrl = card.PictureUrl
    };
}
