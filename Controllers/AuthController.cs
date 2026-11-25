using ProyectoGalactico.Auth;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ProyectoGalactico.Controllers;

// No hay usuarios: lo que se autoriza es un evento. Se identifica por su nombre
// y se autoriza con su código de autorización (guardado como hash en la BDD).
[ApiController]
[Route("autorizacion")]
[Tags("Autorización")]
[ApiExplorerSettings(GroupName = "v1")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("evento")]
    [AllowAnonymous]
    [EnableRateLimiting(AuthSetup.RateLimitPolicy)]
    [EndpointSummary("Autoriza un evento y devuelve un token")]
    [EndpointDescription("Envía el nombre del evento y su código de autorización. El token sirve para simular ese evento.")]
    [ProducesResponseType(typeof(EventAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EventAuthResponseDto>> AuthorizeEvent(EventAuthRequestDto request)
    {
        var authorization = await service.AuthorizeEventAsync(request);

        if (authorization is null)
            return Unauthorized(new ErrorResponse("Nombre de evento o código de autorización incorrectos."));

        return Ok(authorization);
    }

    [HttpGet("evento-actual")]
    [Authorize]
    [EndpointSummary("Muestra el evento autorizado por el token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAuthorizedEvent()
        => Ok(new { eventId = User.GetEventId(), eventName = User.GetEventName() });
}
