using ProyectoGalactico.Dtos.Responses;

namespace ProyectoGalactico.OpenApi
{
    public static class OpenApiExtensions
    {
        // 200 con el dato, o 404 si no existe
        public static RouteHandlerBuilder ProducesRead<T>(this RouteHandlerBuilder b) =>
            b.Produces<T>(StatusCodes.Status200OK)
             .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // 200 con una lista, o 400 si un filtro es inválido
        public static RouteHandlerBuilder ProducesList<T>(this RouteHandlerBuilder b) =>
            b.Produces<List<T>>(StatusCodes.Status200OK)
             .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // 201 al crear, o 400 si los datos son inválidos
        public static RouteHandlerBuilder ProducesCreate<T>(this RouteHandlerBuilder b) =>
            b.Produces<T>(StatusCodes.Status201Created)
             .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // 200, 400 o 404: modificar, MVP y simular
        public static RouteHandlerBuilder ProducesAction<T>(this RouteHandlerBuilder b) =>
            b.Produces<T>(StatusCodes.Status200OK)
             .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
             .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // 204 sin contenido, 400 o 404
        public static RouteHandlerBuilder ProducesDelete(this RouteHandlerBuilder b) =>
            b.Produces(StatusCodes.Status204NoContent)
             .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
             .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}