using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
namespace ProyectoGalactico.Services;

public interface ICharacterService
{
    Task<ServiceResult<List<CharacterDto>>> GetAllAsync(string? faction, bool? sensoryForce);
    Task<ServiceResult<CharacterDto>> GetByIdAsync(int id);
    Task<ServiceResult<CharacterDto>> CreateAsync(CharacterCreateDto dto);
    Task<ServiceResult<CharacterDto>> UpdateAsync(int id, CharacterCreateDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
