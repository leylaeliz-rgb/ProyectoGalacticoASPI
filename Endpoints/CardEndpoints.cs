using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Services;
using ProyectoGalactico.OpenApi;

namespace ProyectoGalactico.Endpoints;

public static class CardEndpoints
{
    public static void MapCardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cartas").WithTags("Cartas");

        group.MapGet("/", async (CardService service, int? characterId, int? minDangerousLevel) =>
                Results.Ok(await service.GetAllAsync(characterId, minDangerousLevel)))
            .WithGroupName("v1").WithSummary("Lista cartas con filtros opcionales")
            .WithDescription("Filtra por personaje (characterId) y por peligrosidad mínima.")
            .Produces<List<CardCharacterDto>>();

        group.MapGet("/{id:int}", async (CardService service, int id) =>
                (await service.GetByIdAsync(id)).ToHttp())
            .WithGroupName("v1").WithSummary("Obtiene una carta por id")
            .ProducesRead<CardCharacterDto>();

        group.MapPost("/", async (CardService service, CardCharacterCreateDto input) =>
        {
            var result = await service.CreateAsync(input);
            return result.Status == ResultStatus.Ok
                ? Results.Created($"/cartas/{result.Value!.Id}", result.Value)
                : result.ToHttp();
        })
            .WithGroupName("v2").WithSummary("Crea una carta para un personaje existente")
            .ProducesCreate<CardCharacterDto>();

        group.MapPut("/{id:int}", async (CardService service, int id, CardCharacterCreateDto input) =>
                (await service.UpdateAsync(id, input)).ToHttp())
            .WithGroupName("v2").WithSummary("Modifica una carta")
            .ProducesAction<CardCharacterDto>();

        group.MapDelete("/{id:int}", async (CardService service, int id) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Status == ResultStatus.Ok ? Results.NoContent() : result.ToHttp();
        })
            .WithGroupName("v2").WithSummary("Elimina una carta")
            .ProducesDelete();
    }
}
