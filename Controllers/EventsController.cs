using Microsoft.AspNetCore.Mvc;
using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;

namespace ProyectoGalactico.Controllers;

[ApiController]
[Route("eventos")]
[Tags("Eventos")]
public class EventsController(IEventService service) : ControllerBase
{
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v1")]
    [EndpointSummary("Lista eventos en orden cronológico")]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? characterId, [FromQuery] string? desde, [FromQuery] string? hasta)
        => (await service.GetAllAsync(characterId, desde, hasta)).ToActionResult(this);

    [HttpGet("{id:int}")]
    [ApiExplorerSettings(GroupName = "v1")]
    [EndpointSummary("Obtiene un evento por id")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => (await service.GetByIdAsync(id)).ToActionResult(this);

    [HttpGet("{id:int}/mvp")]
    [ApiExplorerSettings(GroupName = "v1")]
    [EndpointSummary("Participante con mayor poder del evento")]
    [ProducesResponseType(typeof(MvpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMvp(int id)
        => (await service.GetMvpAsync(id)).ToActionResult(this);

    [HttpGet("/personajes/{id:int}/eventos")]
    [ApiExplorerSettings(GroupName = "v1")]
    [EndpointSummary("Eventos en los que participa un personaje")]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCharacter(int id)
        => (await service.GetByCharacterAsync(id)).ToActionResult(this);

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [EndpointSummary("Crea un evento")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(EventCreateDto dto)
    {
        var result = await service.CreateAsync(dto);
        return result.Status == ResultStatus.Ok
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : result.ToActionResult(this);
    }

    [HttpPut("{id:int}")]
    [ApiExplorerSettings(GroupName = "v2")]
    [EndpointSummary("Modifica un evento")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, EventCreateDto dto)
        => (await service.UpdateAsync(id, dto)).ToActionResult(this);
}