using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Auth;
using ProyectoGalactico.Data;
using ProyectoGalactico.Logging;
using ProyectoGalactico.Models;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Services;

var builder = WebApplication.CreateBuilder(args);
var origenPermitido = builder.Configuration["Cors:Origen"]
    ?? throw new InvalidOperationException("Falta la configuración Cors:Origen.");
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(origenPermitido)
        .AllowAnyHeader()
        .AllowAnyMethod()));
builder.AddGalacticLogging();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 46)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()));

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false)));

builder.Services.AddGalacticOpenApi();

builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IBattleService, BattleService>();
builder.Services.AddScoped<RankingService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IPasswordHasher<Event>, PasswordHasher<Event>>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var client = context.Connection.RemoteIpAddress?.ToString() ?? "desconocido";

        return RateLimitPartition.GetFixedWindowLimiter(client, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });

    options.AddPolicy(AuthSetup.RateLimitPolicy, context =>
    {
        var client = context.Connection.RemoteIpAddress?.ToString() ?? "desconocido";

        return RateLimitPartition.GetFixedWindowLimiter(client, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

builder.Services.AddEventAuthorization(builder.Configuration);
builder.Services.AddGalacticErrorHandling();

var app = builder.Build();

app.UseGalacticLogging();

if (app.Environment.IsDevelopment())
    app.UseGalacticSwagger();

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
