using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Services
{
    public interface ICardService
    {
        Task<ServiceResult<List<CardCharacterDto>>> GetAllAsync(int? characterId, int? minDangerousLevel);
        Task<ServiceResult<CardCharacterDto>> GetByIdAsync(int id);
        Task<CardCharacter?> GetBestCardAsync(int characterId);
        Task<ServiceResult<CardCharacterDto>> CreateAsync(CardCharacterCreateDto dto);
        Task<ServiceResult<CardCharacterDto>> UpdateAsync(int id, CardCharacterCreateDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<string?>> ValidateAsync(CardCharacterCreateDto dto);

    }
}
