using ProyectoGalactico.Common;
using ProyectoGalactico.Models;
using ProyectoGalactico.Services;
using ProyectoGalactico.OpenApi;

namespace ProyectoGalactico.Endpoints
{
    public static class CardEndpoints
    {
        public static void MapCardEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/cartas").WithTags("Cartas");

            group.MapGet("/", (CardService service, int? characterId, int? minDangerousLevel) =>
                    Results.Ok(service.GetAll(characterId, minDangerousLevel)))
                .WithGroupName("v1")
                .WithSummary("Lista cartas con filtros opcionales")
                .WithDescription("Filtra por personaje (characterId) y por peligrosidad mínima.")
                .Produces<List<CardCharacter>>();

            group.MapGet("/{id:int}", (CardService service, int id) =>
                    service.GetById(id).ToHttp())
                .WithGroupName("v1")
                .WithSummary("Obtiene una carta por id")
                .ProducesRead<CardCharacter>();

            group.MapPost("/", (CardService service, CardsCharacterInput input) =>
            {
                var result = service.Create(input);
                return result.Status == ResultStatus.Ok
                    ? Results.Created($"/cartas/{result.Value!.Id}", result.Value)
                    : result.ToHttp();
            })
                .WithGroupName("v2")
                .WithSummary("Crea una carta para un personaje existente")
                .ProducesCreate<CardCharacter>();

            group.MapPut("/{id:int}", (CardService service, int id, CardsCharacterInput input) =>
                    service.Update(id, input).ToHttp())
                .WithGroupName("v2")
                .WithSummary("Modifica una carta")
                .ProducesAction<CardCharacter>();
        }
    }
}