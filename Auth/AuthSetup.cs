using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProyectoGalactico.Services;

namespace ProyectoGalactico.Auth;

public static class AuthSetup
{
    // Política de rate limit para el endpoint de autorización de eventos.
    public const string RateLimitPolicy = "event-auth";

    public static IServiceCollection AddEventAuthorization(
        this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Clave"]
            ?? throw new InvalidOperationException("Falta el secreto Jwt:Clave");

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Emisor"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audiencia"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = EventClaims.EventName
                };
            });

        services.AddAuthorization();
        return services;
    }
}
