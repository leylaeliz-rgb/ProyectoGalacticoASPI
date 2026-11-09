using Microsoft.AspNetCore.Mvc;
using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;
using static ProyectoGalactico.Dtos.Responses.RankingResponses;

namespace ProyectoGalactico.Controllers;

[ApiController]
[Tags("Juego")]
[ApiExplorerSettings(GroupName = "v1")]
public class GameController(BattleService battle, RankingService ranking) : ControllerBase
{
    [HttpPost("/eventos/{id:int}/simular")]
    [EndpointSummary("Simula la batalla de un evento")]
    [EndpointDescription("Autobot contra Decepticon; los neutrales dan apoyo. Guarda el ganador cifrado en el evento.")]
    [ProducesResponseType(typeof(BattleResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Simulate(int id, [FromQuery] int? seed)
        => (await battle.SimulateAsync(id, seed)).ToActionResult(this);

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