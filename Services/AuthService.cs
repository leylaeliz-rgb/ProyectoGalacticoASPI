using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ProyectoGalactico.Services
{
    public interface IAuthService
    {
        Task<EventAuthResponseDto?> AuthorizeEventAsync(EventAuthRequestDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<Event> _hasher;
        private readonly ITokenService _tokens;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext db, IPasswordHasher<Event> hasher, ITokenService tokens, ILogger<AuthService> logger)
        {
            _db = db;
            _hasher = hasher;
            _tokens = tokens;
            _logger = logger;
        }

        public async Task<EventAuthResponseDto?> AuthorizeEventAsync(EventAuthRequestDto request)
        {
            // El nombre no es único: se prueba el código contra todos los eventos con ese nombre.
            var name = request.Name.Trim();
            var candidates = await _db.Events.AsNoTracking()
                .Where(e => e.Name == name)
                .OrderBy(e => e.Id)
                .ToListAsync();

            if (candidates.Count == 0)
            {
                // No se registra el nombre: lo escribe quien consulta y no hay evento que lo respalde.
                _logger.LogWarning("Autorización fallida: no existe un evento con el nombre indicado");
                return null;
            }

            var eventEntity = candidates.FirstOrDefault(e =>
                _hasher.VerifyHashedPassword(e, e.HashAutorizationCode, request.AutorizationCode)
                    != PasswordVerificationResult.Failed);

            if (eventEntity is null)
            {
                _logger.LogWarning(
                    "Autorización fallida: código incorrecto para {CandidateCount} evento(s) (primer id {EventId})",
                    candidates.Count, candidates[0].Id);
                return null;
            }

            _logger.LogInformation("Evento {EventId} autorizado", eventEntity.Id);
            var (token, expiresAt) = _tokens.Generate(eventEntity);

            return new EventAuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                EventName = eventEntity.Name,
            };
        }
    }
}
