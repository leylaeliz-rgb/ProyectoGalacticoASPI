using ProyectoGalactico.Common;
using ProyectoGalactico.Models;
using ProyectoGalactico.Services;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Dtos.Responses;
namespace ProyectoGalactico.Endpoints
{
    public static class EventEndpoints
    {
        public static void MapEventEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/eventos").WithTags("Eventos");

            group.MapGet("/", (EventService service, int? characterId, string? desde, string? hasta) =>
                    service.GetAll(characterId, desde, hasta).ToHttp())
                .WithGroupName("v1")
                .WithSummary("Lista eventos en orden cronológico")
                .WithDescription("Filtra por personaje (characterId) y por rango de fechas: desde=10 BBY&hasta=3 ABY.")
                .ProducesList<EventResponse>();

            group.MapGet("/{id:int}", (EventService service, int id) =>
                    service.GetById(id).ToHttp())
                .WithGroupName("v1")
                .WithSummary("Obtiene un evento por id")
                .ProducesRead<EventResponse>();

            group.MapGet("/{id:int}/mvp", (EventService service, int id) =>
                    service.GetMvp(id).ToHttp())
                .WithGroupName("v1")
                .WithSummary("Participante con mayor poder del evento")
                .WithDescription("Poder = peligrosidad de su mejor carta + bono por sensibilidad a la Fuerza. En empate gana el de menor id.")
                .ProducesAction<MvpResponse>();

            group.MapPost("/", (EventService service, EventInput input) =>
            {
                var result = service.Create(input);
                return result.Status == ResultStatus.Ok
                    ? Results.Created($"/eventos/{result.Value!.Id}", result.Value)
                    : result.ToHttp();
            })
                .WithGroupName("v2")
                .WithSummary("Crea un evento")
                .WithDescription("La fecha va como texto: '10 BBY', '3 ABY' o 'Batalla de Yavin'. Los ids en deadIds pasan a estado muerto.")
                .ProducesCreate<EventResponse>();

            group.MapPut("/{id:int}", (EventService service, int id, EventInput input) =>
                    service.Update(id, input).ToHttp())
                .WithGroupName("v2")
                .WithSummary("Modifica un evento")
                .ProducesAction<EventResponse>();

            // Relación personaje -> eventos
            app.MapGet("/personajes/{id:int}/eventos", (EventService service, int id) =>
                    service.GetByCharacter(id).ToHttp())
                .WithTags("Personajes")
                .WithGroupName("v1")
                .WithSummary("Eventos en los que participa un personaje")
                .ProducesRead<List<EventResponse>>();
        }
    }
}