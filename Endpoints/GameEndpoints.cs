using ProyectoGalactico.Common;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Services;
using static ProyectoGalactico.Dtos.Responses.RankingResponses;

namespace ProyectoGalactico.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/eventos/{id:int}/simular", async (BattleService service, int id, int? seed) =>
                (await service.SimulateAsync(id, seed)).ToHttp())
            .WithTags("Juego").WithGroupName("v1")
            .WithSummary("Simula la batalla de un evento")
            .WithDescription("Autobot contra Decepticon; los neutrales dan apoyo. Usa ?seed=N para repetir exactamente el mismo resultado. Guarda el ganador cifrado en el evento.")
            .ProducesAction<BattleResult>();

        app.MapGet("/personajes/ranking", async (RankingService service, string? por, string? faction) =>
                (await service.GetRankingAsync(por, faction)).ToHttp())
            .WithTags("Juego").WithGroupName("v1")
            .WithSummary("Ranking de personajes por poder")
            .WithDescription("Poder = peligrosidad de la mejor carta + bono de sensibilidad. Solo entran personajes con carta. Filtro opcional: faction.")
            .ProducesList<RankingItem>();
    }
}
