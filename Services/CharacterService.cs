using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Enums;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Services;

public class CharacterService(AppDbContext db) : ICharacterService
{
    private readonly AppDbContext _db = db;

    public async Task<ServiceResult<List<CharacterDto>>> GetAllAsync(string? faction, bool? sensoryForce)
    {
        Faction? parsedFaction = null;
        if (!string.IsNullOrWhiteSpace(faction))
        {
            if (!Enum.TryParse<Faction>(faction, true, out var value) || !Enum.IsDefined(value))
                return ServiceResult<List<CharacterDto>>.Invalid("Facción no válida. Usa Autobot, Decepticon o Neutral.");
            parsedFaction = value;
        }

        var query = _db.Characters.AsNoTracking().Include(c => c.Cards).AsQueryable();
        if (parsedFaction.HasValue) query = query.Where(c => c.Faction == parsedFaction.Value);
        if (sensoryForce.HasValue) query = query.Where(c => c.SensoryForce == sensoryForce.Value);

        var characters = await query.OrderBy(c => c.Id).ToListAsync();
        return ServiceResult<List<CharacterDto>>.Ok(characters.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<CharacterDto>> GetByIdAsync(int id)
    {
        var character = await _db.Characters.AsNoTracking()
            .Include(c => c.Cards).FirstOrDefaultAsync(c => c.Id == id);
        return character is null
            ? ServiceResult<CharacterDto>.NotFound($"No existe el personaje {id}.")
            : ServiceResult<CharacterDto>.Ok(ToDto(character));
    }

    public async Task<ServiceResult<CharacterDto>> CreateAsync(CharacterCreateDto dto)
    {
        if (dto.Faction is null || !Enum.IsDefined(dto.Faction.Value))
            return ServiceResult<CharacterDto>.Invalid("La facción no es válida.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResult<CharacterDto>.Invalid("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Species))
            return ServiceResult<CharacterDto>.Invalid("La especie es obligatoria.");

        var character = new Character
        {
            Name = dto.Name.Trim(),
            Species = dto.Species.Trim(),
            Faction = dto.Faction.Value,
            CharacterState = CharacterState.Alive,
            IsActive = true,
            SensoryForce = dto.SensoryForce
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();
        return ServiceResult<CharacterDto>.Ok(ToDto(character));
    }

    public async Task<ServiceResult<CharacterDto>> UpdateAsync(int id, CharacterCreateDto dto)
    {
        var character = await _db.Characters.Include(c => c.Cards)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (character is null)
            return ServiceResult<CharacterDto>.NotFound($"No existe el personaje {id}.");

        if (dto.Faction is null || !Enum.IsDefined(dto.Faction.Value))
            return ServiceResult<CharacterDto>.Invalid("La facción no es válida.");
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Species))
            return ServiceResult<CharacterDto>.Invalid("El nombre y la especie son obligatorios.");

        character.Name = dto.Name.Trim();
        character.Species = dto.Species.Trim();
        character.Faction = dto.Faction.Value;
        character.SensoryForce = dto.SensoryForce;

        await _db.SaveChangesAsync();
        return ServiceResult<CharacterDto>.Ok(ToDto(character));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var character = await _db.Characters.FirstOrDefaultAsync(c => c.Id == id);
        if (character is null)
            return ServiceResult<bool>.NotFound($"No existe el personaje {id}.");

        var appearsInEvent = (await _db.Events.AsNoTracking().ToListAsync())
            .Any(e => e.ParticipantsIds.Contains(id));
        if (appearsInEvent)
            return ServiceResult<bool>.Conflict("No se puede eliminar: el personaje participa en eventos.");

        _db.Characters.Remove(character);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private static CharacterDto ToDto(Character character) => new()
    {
        Id = character.Id,
        Name = character.Name,
        Species = character.Species,
        Faction = character.Faction,
        CharacterState = character.CharacterState,
        IsActive = character.IsActive,
        SensoryForce = character.SensoryForce,
        Cards = character.Cards.Select(c => new CardCharacterDto
        {
            Id = c.Id,
            CharacterId = c.CharacterId,
            Power = c.Power,
            SpecialHability = c.SpecialHability,
            Weapon = c.Weapon,
            DangerousLevel = c.DangerousLevel,
            PictureUrl = c.PictureUrl,
            SurvivalRatio = c.SurvivalRatio,
            IsAvailable = c.IsAvailable
        }).ToList()
    };
}
