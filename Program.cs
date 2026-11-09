using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Data;
using ProyectoGalactico.Models;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthorization();
builder.Services.AddGalacticOpenApi();

builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<RankingService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IPasswordHasher<Event>, PasswordHasher<Event>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseGalacticSwagger();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.MapGet("/health/db", async (AppDbContext db) =>
    await db.Database.CanConnectAsync()
        ? Results.Ok(new { status = "ok", database = "connected" })
        : Results.Problem("No fue posible conectar con la base de datos."));

app.Run();
