using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos.Responses;

namespace ProyectoGalactico.Services;

public interface IBattleService
{
    // Simular una batalla
    Task<ServiceResult<BattleResult>> SimulateAsync(int eventId, int? seed);

    // Consultar el historial de un personaje
    Task<ServiceResult<List<BattleHistoryItem>>> GetHistoryAsync(int characterId);
}