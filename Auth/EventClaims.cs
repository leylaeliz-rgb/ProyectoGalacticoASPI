using System.Security.Claims;

namespace ProyectoGalactico.Auth;

// Claims del token: el sujeto autorizado es un evento, no un usuario.
public static class EventClaims
{
    public const string EventId = "sub";
    public const string EventName = "name";

    public static int? GetEventId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(EventId), out var id) ? id : null;

    public static string? GetEventName(this ClaimsPrincipal user) =>
        user.FindFirstValue(EventName);
}
