using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyectoGalactico.Logging;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Excepción no controlada en {Method} {Path}",
            context.Request.Method,
            context.Request.Path.Value);

        var status = exception is DbUpdateException
            ? StatusCodes.Status409Conflict
            : StatusCodes.Status500InternalServerError;

        var title = status == StatusCodes.Status409Conflict
            ? "No se pudo guardar: conflicto con datos existentes"
            : "Ocurrió un error interno";

        context.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Extensions =
                {
                    ["correlationId"] = context.Response.Headers[RequestContextMiddleware.CorrelationHeader].ToString()
                }
            }
        });
    }
}
