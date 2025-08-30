using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace pacientes_service.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _cfg;
        public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

        public string GenerateAccessToken(Usuario u)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 15;
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, u.IdUsuario.ToString()),
                new Claim("username", u.NombreUsuario),
                new Claim(ClaimTypes.Role, u.Rol),
                new Claim("id_medico", u.IdMedico?.ToString() ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string token, string hash, DateTime expiresAt) GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            var token = Base64UrlEncoder.Encode(bytes);
            var days = int.TryParse(_cfg["Jwt:RefreshTokenDays"], out var d) ? d : 7;
            var expires = DateTime.UtcNow.AddDays(days);
            var hash = BCryptNet.HashPassword(token, workFactor: 11);
            return (token, hash, expires);
        }
    }
}
