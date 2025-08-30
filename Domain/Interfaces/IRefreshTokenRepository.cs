using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken rt, CancellationToken ct);
        Task<RefreshToken?> FindValidAsync(int userId, string plainToken, CancellationToken ct); // compara contra hash
        Task RevokeAsync(RefreshToken rt, CancellationToken ct);
    }
}
