using Microsoft.EntityFrameworkCore;
using ProyectoGalactico.Data;
using ProyectoGalactico.Endpoints;
using ProyectoGalactico.OpenApi;
using ProyectoGalactico.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGalacticOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión DefaultConnection");

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySql(
//        connectionString,
//        new MySqlServerVersion(new Version(8, 0, 36)))); // Deben colocar la version de MySQL que tengan instalada en su máquina

builder.Services.AddControllers();
builder.Services.AddOpenApi();
//builder.Services.AddScoped<ICategoriaService, CategoriaService>();
//builder.Services.AddScoped<IProductoService, ProductoService>();

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