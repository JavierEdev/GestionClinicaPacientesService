using Microsoft.AspNetCore.Identity.Data;
using pacientes_service.Communication.Commands;
using pacientes_service.Communication.Contracts;
using pacientes_service.Domain.Interfaces;
using RefreshRequest = pacientes_service.Communication.Commands.RefreshRequest;
using LoginRequest = pacientes_service.Communication.Commands.LoginRequest;

namespace pacientes_service.Communication.Services;

public class AuthService
{
    private readonly IUsuarioRepository _users;
    private readonly ISecurePasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenRepository _refresh;
    private readonly IUnitOfWork _uow;

    public AuthService(
        IUsuarioRepository users,
        ISecurePasswordHasher hasher,
        IJwtTokenService jwt,
        IRefreshTokenRepository refresh,
        IUnitOfWork uow)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _refresh = refresh;
        _uow = uow;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var u = await _users.GetByNombreAsync(req.Username, ct);
        if (u is null) return null;
        if (!_hasher.Verify(req.Password, u.Contrasena)) return null;

        var access = _jwt.GenerateAccessToken(u);
        var (rtPlain, rtHash, exp) = _jwt.GenerateRefreshToken();

        await _refresh.AddAsync(new Domain.Entities.RefreshToken
        {
            IdUsuario = u.IdUsuario,
            TokenHash = rtHash,
            ExpiresAt = exp,
            CreatedAt = DateTime.UtcNow
        }, ct);
        await _uow.SaveChangesAsync(ct);

        return new LoginResponse
        {
            AccessToken = access,
            RefreshToken = rtPlain,
            UserId = u.IdUsuario,
            Username = u.NombreUsuario,
            Role = u.Rol
        };
    }

    public async Task<LoginResponse?> RefreshAsync(RefreshRequest req, CancellationToken ct)
    {
        var valid = await _refresh.FindValidAsync(req.UserId, req.RefreshToken, ct);
        if (valid is null) return null;

        var u = await _users.GetByIdAsync(req.UserId, ct);
        if (u is null) return null;

        await _refresh.RevokeAsync(valid, ct);

        var (rtPlain, rtHash, exp) = _jwt.GenerateRefreshToken();
        await _refresh.AddAsync(new Domain.Entities.RefreshToken
        {
            IdUsuario = u.IdUsuario,
            TokenHash = rtHash,
            ExpiresAt = exp,
            CreatedAt = DateTime.UtcNow
        }, ct);
        await _uow.SaveChangesAsync(ct);

        var access = _jwt.GenerateAccessToken(u);
        return new LoginResponse
        {
            AccessToken = access,
            RefreshToken = rtPlain,
            UserId = u.IdUsuario,
            Username = u.NombreUsuario,
            Role = u.Rol
        };
    }

    public async Task LogoutAsync(int userId, string refreshToken, CancellationToken ct)
    {
        var valid = await _refresh.FindValidAsync(userId, refreshToken, ct);
        if (valid is null) return;
        await _refresh.RevokeAsync(valid, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
