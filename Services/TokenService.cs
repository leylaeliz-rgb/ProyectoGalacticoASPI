using System.Security.Claims;
using System.Text;
using ProyectoGalactico.Auth;
using ProyectoGalactico.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ProyectoGalactico.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) Generate(Event eventEntity);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiresAt) Generate(Event eventEntity)
        {
            var key = _configuration["Jwt:Clave"]
                ?? throw new InvalidOperationException("Falta el secreto Jwt:Clave");

            var minutes = _configuration.GetValue<int?>("Jwt:MinutosExpiracion") ?? 30;
            var expiresAt = DateTime.UtcNow.AddMinutes(minutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(EventClaims.EventId, eventEntity.Id.ToString()),
                    new Claim(EventClaims.EventName, eventEntity.Name)
                }),
                Expires = expiresAt,
                Issuer = _configuration["Jwt:Emisor"],
                Audience = _configuration["Jwt:Audiencia"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (token, expiresAt);
        }
    }
}
