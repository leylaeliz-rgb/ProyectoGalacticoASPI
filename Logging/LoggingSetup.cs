using Serilog;

namespace ProyectoGalactico.Logging;

public static class LoggingSetup
{
    public static WebApplicationBuilder AddGalacticLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext());

        return builder;
    }

    public static IServiceCollection AddGalacticErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }

    // Debe ir primero en el pipeline: así toda petición queda con correlation id
    // y las excepciones llegan al manejador global con la respuesta ya preparada.
    public static WebApplication UseGalacticLogging(this WebApplication app)
    {
        app.UseMiddleware<RequestContextMiddleware>();
        app.UseExceptionHandler();
        return app;
    }
}
