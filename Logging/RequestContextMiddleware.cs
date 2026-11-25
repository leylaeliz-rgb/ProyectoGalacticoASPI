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
