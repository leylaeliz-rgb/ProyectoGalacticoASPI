using Microsoft.AspNetCore.Mvc;
using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;

namespace ProyectoGalactico.Controllers;

[ApiController]
[Route("cartas")]
[Tags("Cartas")]
public class CardsController(CardService service) : ControllerBase
{
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v1")]
    [EndpointSummary("Lista cartas con filtros opcionales")]
    [ProducesResponseType(typeof(List<CardCharacterDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? characterId, [FromQuery] int? minDangerousLevel)
        => Ok(await service.GetAllAsync(characterId, minDangerousLevel));

    [HttpGet("{id:int}")]
    [ApiExplorerSettings(GroupName = "v1")]
    [EndpointSummary("Obtiene una carta por id")]
    [ProducesResponseType(typeof(CardCharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => (await service.GetByIdAsync(id)).ToActionResult(this);

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [EndpointSummary("Crea una carta para un personaje existente")]
    [ProducesResponseType(typeof(CardCharacterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CardCharacterCreateDto dto)
    {
        var result = await service.CreateAsync(dto);
        return result.Status == ResultStatus.Ok
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : result.ToActionResult(this);
    }

    [HttpPut("{id:int}")]
    [ApiExplorerSettings(GroupName = "v2")]
    [EndpointSummary("Modifica una carta")]
    [ProducesResponseType(typeof(CardCharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, CardCharacterCreateDto dto)
        => (await service.UpdateAsync(id, dto)).ToActionResult(this);

    [HttpDelete("{id:int}")]
    [ApiExplorerSettings(GroupName = "v2")]
    [EndpointSummary("Elimina una carta")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await service.DeleteAsync(id);
        return result.Status == ResultStatus.Ok ? NoContent() : result.ToActionResult(this);
    }
}