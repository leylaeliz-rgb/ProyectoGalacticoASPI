using System.Security.Claims;

namespace ProyectoGalactico.Auth;

// Claims del token: el sujeto autorizado es un evento, no un usuario.
// define que datos lleva el token y se utiliza en game controller para comparar el
//evento del token con el de la ruta
public static class EventClaims
{
    public const string EventId = "sub";
    public const string EventName = "name";
    public const string Role = "role";
    public const string AdminRole = "Administrador";
    public const string AdminSubject = "admin";

    public static int? GetEventId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(EventId), out var id) ? id : null;

    public static string? GetEventName(this ClaimsPrincipal user) =>
        user.FindFirstValue(EventName);
}
