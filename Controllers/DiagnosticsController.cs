using Microsoft.AspNetCore.Mvc;

namespace ProyectoGalactico.Controllers;

// Solo para comprobar el manejo global de errores y los logs. No existe fuera de Development.
[ApiController]
[Route("diagnostico")]
[ApiExplorerSettings(IgnoreApi = true)]
public class DiagnosticsController(IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("error")]
    public IActionResult ForceError()
    {
        if (!environment.IsDevelopment())
            return NotFound();

        throw new InvalidOperationException("Error de prueba provocado a propósito.");
    }
}
