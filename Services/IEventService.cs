using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
namespace ProyectoGalactico.Services;

public interface IEventService
{
    Task<ServiceResult<List<EventDto>>> GetAllAsync(int? characterId, string? from, string? to);
    Task<ServiceResult<EventDto>> GetByIdAsync(int id);
    Task<ServiceResult<List<EventDto>>> GetByCharacterAsync(int characterId);
    Task<ServiceResult<EventDto>> CreateAsync(EventCreateDto dto);
    Task<ServiceResult<EventDto>> UpdateAsync(int id, EventCreateDto dto);
    Task<ServiceResult<MvpResponse>> GetMvpAsync(int eventId);
}
