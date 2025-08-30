using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Usuario user);
        (string token, string hash, DateTime expiresAt) GenerateRefreshToken();
    }
}
