using pacientes_service.Communication.Contracts;
using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Communication.Services;

public class GetUserByIdService
{
    private readonly IUsuarioRepository _repo;
    public GetUserByIdService(IUsuarioRepository repo) => _repo = repo;

    public async Task<UserResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        return u is null ? null : new UserResponse { Id = u.IdUsuario, Username = u.NombreUsuario, Rol = u.Rol, IdMedico = u.IdMedico };
    }
}
