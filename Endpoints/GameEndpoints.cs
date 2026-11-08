using ProyectoGalactico.Common;
using ProyectoGalactico.Services;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Dtos.Responses;
namespace ProyectoGalactico.Endpoints
{
    public static class GameEndpoints
    {
        public static void MapGameEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/eventos/{id:int}/simular", (BattleService service, int id, int? seed) =>
                    service.Simulate(id, seed).ToHttp())
                .WithTags("Juego")
                .WithGroupName("v1")
                .WithSummary("Simula la batalla de un evento")
                .WithDescription("Rebelde contra Imperio; los neutrales dan apoyo. Usa ?seed=N para repetir exactamente el mismo resultado. Guarda el ganador en el evento.")
                .ProducesAction<BattleResult>();

            app.MapGet("/personajes/ranking", (RankingService service, string? por, string? faccion) =>
                    service.GetRanking(por, faccion).ToHttp())
                .WithTags("Juego")
                .WithGroupName("v1")
                .WithSummary("Ranking de personajes por poder")
                .WithDescription("Poder = peligrosidad de la mejor carta + bono de sensibilidad. Solo entran personajes con carta. Filtro opcional: faccion.")
                .ProducesList<RankingItem>();
        }
    }
}