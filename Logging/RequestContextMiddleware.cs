using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using ProyectoGalactico.Auth;
using Serilog.Context;

namespace ProyectoGalactico.Logging;

public class RequestContextMiddleware(RequestDelegate next, ILogger<RequestContextMiddleware> logger)
{
    public const string CorrelationHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        context.Response.Headers[CorrelationHeader] = correlationId;

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "desconocido";
        var userAgent = context.Request.Headers.UserAgent.ToString();
        if (userAgent.Length > 200)
        {
            userAgent = userAgent[..200];
        }
        var fingerprint = ComputeFingerprint(ip, userAgent);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("ClientIp", ip))
        using (LogContext.PushProperty("UserAgent", userAgent))
        using (LogContext.PushProperty("Fingerprint", fingerprint))
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        logger.LogWarning("Acceso denegado (403) a {Method} {Path} (evento {AuthorizedEventId})",
                            context.Request.Method, context.Request.Path.Value, authorizedEventId);
                    }
                stopwatch.Stop();
                var authorizedEventId = context.User.GetEventId()?.ToString() ?? "anónimo";

                logger.LogInformation(
                    "{Method} {Path} respondió {StatusCode} en {ElapsedMs} ms (evento {AuthorizedEventId})",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    authorizedEventId);
            }
        }
    }

    private static string ComputeFingerprint(string ip, string userAgent)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{ip}|{userAgent}"));
        return Convert.ToHexString(bytes)[..12].ToLowerInvariant();
    }
}

//RequestContextMiddleware.cs corre en cada petición:
//Genera un id de 8 caracteres y 
//lo pone en la cabecera X-Correlation - Id de la respuesta.
//Toma la IP y el User-Agent(recortado a 200 caracteres) 
//y calcula una huella SHA - 256 corta de ambos.
//Mete todo eso en el contexto de Serilog, así cada 
//log de esa petición lleva esos datos.
//Mide el tiempo con un cronómetro y, en el finally, 
//escribe una línea con método, ruta, estado, milisegundos
//y el id de evento autorizado (o "anónimo"). Va en finally para registrar
//la petición incluso si hubo un error, y porque ahí el token ya fue validado.