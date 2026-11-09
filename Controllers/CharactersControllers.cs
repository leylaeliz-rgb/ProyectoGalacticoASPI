using Microsoft.AspNetCore.Mvc;
using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;

namespace ProyectoGalactico.Controllers;

[ApiController]
[Route("personajes")]
public class CharactersController(ICharacterService service) : ControllerBase
{
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v1")]
    [ProducesResponseType(typeof(List<CharacterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] string? faction, [FromQuery] bool? sensoryForce)
        => (await service.GetAllAsync(faction, sensoryForce)).ToActionResult(this);

    [HttpGet("{id:int}")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => (await service.GetByIdAsync(id)).ToActionResult(this);

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CharacterCreateDto dto)
    {
        var result = await service.CreateAsync(dto);
        return result.Status == ResultStatus.Ok
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : result.ToActionResult(this);
    }

    [HttpPut("{id:int}")]
    [ApiExplorerSettings(GroupName = "v2")]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, CharacterCreateDto dto)
        => (await service.UpdateAsync(id, dto)).ToActionResult(this);

    [HttpDelete("{id:int}")]
    [ApiExplorerSettings(GroupName = "v2")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await service.DeleteAsync(id);
        return result.Status == ResultStatus.Ok ? NoContent() : result.ToActionResult(this);
    }
}