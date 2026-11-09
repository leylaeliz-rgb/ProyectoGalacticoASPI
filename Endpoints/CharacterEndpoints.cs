using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Services;
using ProyectoGalactico.OpenApi;

namespace ProyectoGalactico.Endpoints;

public static class CharacterEndpoints
{
    public static void MapCharacterEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/personajes").WithTags("Personajes");

        group.MapGet("/", async (ICharacterService service, string? faction, bool? sensoryForce) =>
                (await service.GetAllAsync(faction, sensoryForce)).ToHttp())
            .WithGroupName("v1")
            .WithSummary("Lista personajes con filtros opcionales")
            .WithDescription("Filtra por facción (Rebelde, Imperio, Neutral) y por sensibilidad a la Fuerza.")
            .ProducesList<CharacterDto>();

        group.MapGet("/{id:int}", async (ICharacterService service, int id) =>
                (await service.GetByIdAsync(id)).ToHttp())
            .WithGroupName("v1").WithSummary("Obtiene un personaje por id")
            .ProducesRead<CharacterDto>();

        group.MapPost("/", async (ICharacterService service, CharacterCreateDto input) =>
        {
            var result = await service.CreateAsync(input);
            return result.Status == ResultStatus.Ok
                ? Results.Created($"/personajes/{result.Value!.Id}", result.Value)
                : result.ToHttp();
        })
            .WithGroupName("v2").WithSummary("Crea un personaje")
            .ProducesCreate<CharacterDto>();

        group.MapPut("/{id:int}", async (ICharacterService service, int id, CharacterCreateDto input) =>
                (await service.UpdateAsync(id, input)).ToHttp())
            .WithGroupName("v2").WithSummary("Modifica un personaje")
            .ProducesAction<CharacterDto>();

        group.MapDelete("/{id:int}", async (ICharacterService service, int id) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Status == ResultStatus.Ok ? Results.NoContent() : result.ToHttp();
        })
            .WithGroupName("v2").WithSummary("Elimina un personaje")
            .ProducesDelete();
    }
}
