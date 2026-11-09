using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;

public interface ICardService
{
    Task<List<CardCharacterDto>> GetAllAsync(int? characterId, int? minDangerousLevel);
    Task<ServiceResult<CardCharacterDto>> GetByIdAsync(int id);
    Task<CardCharacter?> GetBestCardAsync(int characterId);
    Task<ServiceResult<CardCharacterDto>> CreateAsync(CardCharacterCreateDto dto);
    Task<ServiceResult<CardCharacterDto>> UpdateAsync(int id, CardCharacterCreateDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}