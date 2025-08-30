using pacientes_service.Communication.Contracts;
using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Communication.Services;

public class ListUsersService
{
    private readonly IUsuarioRepository _repo;
    public ListUsersService(IUsuarioRepository repo) => _repo = repo;

    // ahora recibe el filtro opcional
    public async Task<List<UserResponse>> ExecuteAsync(int? pacienteId, CancellationToken ct)
    {
        var usuarios = await _repo.GetAllAsync(pacienteId, ct);

        return usuarios
            .Select(u => new UserResponse
            {
                Id = u.IdUsuario,
                Username = u.NombreUsuario,
                Rol = u.Rol,
                IdMedico = u.IdMedico,
                IdPaciente = u.IdPaciente
            })
            .ToList();
    }
}
