using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;
using ProyectoGalactico.OpenApi;

namespace ProyectoGalactico.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/eventos").WithTags("Eventos");

        group.MapGet("/", async (IEventService service, int? characterId, string? desde, string? hasta) =>
                (await service.GetAllAsync(characterId, desde, hasta)).ToHttp())
            .WithGroupName("v1").WithSummary("Lista eventos en orden cronológico")
            .WithDescription("Filtra por personaje (characterId) y por rango de fechas: desde=10 BBY&hasta=3 ABY.")
            .ProducesList<EventDto>();

        group.MapGet("/{id:int}", async (IEventService service, int id) =>
                (await service.GetByIdAsync(id)).ToHttp())
            .WithGroupName("v1").WithSummary("Obtiene un evento por id")
            .ProducesRead<EventDto>();

        group.MapGet("/{id:int}/mvp", async (IEventService service, int id) =>
                (await service.GetMvpAsync(id)).ToHttp())
            .WithGroupName("v1").WithSummary("Participante con mayor poder del evento")
            .WithDescription("Poder = peligrosidad de su mejor carta + bono por sensibilidad a la Fuerza. En empate gana el de menor id.")
            .ProducesAction<MvpResponse>();

        group.MapPost("/", async (IEventService service, EventCreateDto input) =>
        {
            var result = await service.CreateAsync(input);
            return result.Status == ResultStatus.Ok
                ? Results.Created($"/eventos/{result.Value!.Id}", result.Value)
                : result.ToHttp();
        })
            .WithGroupName("v2").WithSummary("Crea un evento")
            .WithDescription("El año se envía como entero: negativo para BBY, cero para la Batalla de Yavin y positivo para ABY. Los ids en deadIds pasan a estado muerto.")
            .ProducesCreate<EventDto>();

        group.MapPut("/{id:int}", async (IEventService service, int id, EventCreateDto input) =>
                (await service.UpdateAsync(id, input)).ToHttp())
            .WithGroupName("v2").WithSummary("Modifica un evento")
            .ProducesAction<EventDto>();

        app.MapGet("/personajes/{id:int}/eventos", async (IEventService service, int id) =>
                (await service.GetByCharacterAsync(id)).ToHttp())
            .WithTags("Personajes").WithGroupName("v1")
            .WithSummary("Eventos en los que participa un personaje")
            .ProducesRead<List<EventDto>>();
    }
}
