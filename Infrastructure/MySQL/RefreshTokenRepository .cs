using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Infrastructure.MySQL;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;
    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public Task AddAsync(RefreshToken rt, CancellationToken ct) =>
        _db.Set<RefreshToken>().AddAsync(rt, ct).AsTask();

    // Trae vigentes del usuario y verifica el hash en memoria (simple y suficiente)
    public async Task<RefreshToken?> FindValidAsync(int userId, string plainToken, CancellationToken ct)
    {
        var candidates = await _db.Set<RefreshToken>()
            .Where(x => x.IdUsuario == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        return candidates.FirstOrDefault(x => BCrypt.Net.BCrypt.Verify(plainToken, x.TokenHash));
    }

    public Task RevokeAsync(RefreshToken rt, CancellationToken ct)
    {
        _db.Attach(rt);
        rt.RevokedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}
