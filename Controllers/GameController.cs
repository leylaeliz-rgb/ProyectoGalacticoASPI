using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGalactico.Auth;
using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;
using static ProyectoGalactico.Dtos.Responses.RankingResponses;

namespace ProyectoGalactico.Controllers;

[ApiController]
[Tags("Juego")]
[ApiExplorerSettings(GroupName = "v1")]
public class GameController(IBattleService battle, RankingService ranking, ILogger<GameController> logger) : ControllerBase
{
    [HttpPost("/eventos/{id:int}/simular")]
    [Authorize]
    [EndpointSummary("Simula la batalla de un evento")]
    [EndpointDescription("Autobot contra Decepticon; los neutrales dan apoyo. Guarda el ganador cifrado en el evento. Requiere el token del mismo evento (POST /autorizacion/evento).")]
    [ProducesResponseType(typeof(BattleResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Simulate(int id, [FromQuery] int? seed)
    {
        var authorizedEventId = User.GetEventId();
        if (authorizedEventId != id)
        {
            logger.LogWarning(
                "El token del evento {AuthorizedEventId} no autoriza simular el evento {EventId}",
                authorizedEventId, id);
            return ServiceResult<BattleResult>
                .Forbidden("El token no autoriza la simulación de este evento.")
                .ToActionResult(this);
        }

        return (await battle.SimulateAsync(id, seed)).ToActionResult(this);
    }

    [HttpGet("/personajes/ranking")]
    [EndpointSummary("Ranking de personajes por poder")]
    [ProducesResponseType(typeof(List<RankingItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRanking([FromQuery] string? por, [FromQuery] string? faction)
        => (await ranking.GetRankingAsync(por, faction)).ToActionResult(this);

    [HttpGet("/personajes/{id:int}/historial")]
    [EndpointSummary("Consulta el historial de batallas de un personaje")]
    [EndpointDescription("Devuelve los eventos en los que participó el personaje y las consecuencias de cada batalla.")]
    [ProducesResponseType(typeof(List<BattleHistoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int id)
    => (await battle.GetHistoryAsync(id)).ToActionResult(this);
}