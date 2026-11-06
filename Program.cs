using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using ProyectoGalactico.Data;
using ProyectoGalactico.Endpoints;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGalacticOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 42)), //version mysql
        MySqlOptions => MySqlOptions.EnableRetryOnFailure())); // Deben colocar la version de MySQL que tengan instalada en su máquina

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false)));
builder.Services.AddOpenApi();
//builder.Services.AddScoped<ICategoriaService, CategoriaService>();
//builder.Services.AddScoped<IProductoService, ProductoService>();
// evitar que la cadena de conexión y cifrado clave se vayan al git o repositorio
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseGalacticSwagger();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapCharacterEndpoints();
app.MapCardEndpoints();
app.MapEventEndpoints();
app.MapGameEndpoints();

app.Run();