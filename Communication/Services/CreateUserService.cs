using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Communication.Services;

public class CreateUserService
{
    private readonly IUsuarioRepository _repo;
    private readonly ISecurePasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public CreateUserService(IUsuarioRepository repo, ISecurePasswordHasher hasher, IUnitOfWork uow)
    {
        _repo = repo; _hasher = hasher; _uow = uow;
    }

    public async Task<int> ExecuteAsync(CreateUserCommand cmd, CancellationToken ct)
    {
        var rolesValidos = new[] { "administrador", "medico", "recepcionista" };
        if (!rolesValidos.Contains(cmd.Rol))
            throw new InvalidOperationException("Rol inválido");

        if (await _repo.ExistsByUsernameAsync(cmd.Username, ct))
            throw new InvalidOperationException("El nombre de usuario ya existe");

        var entity = new Usuario
        {
            NombreUsuario = cmd.Username.Trim(),
            Contrasena = _hasher.Hash(cmd.Password),
            Rol = cmd.Rol,
            IdMedico = cmd.IdMedico,
            IdPaciente = cmd.IdPaciente
        };

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.IdUsuario;
    }
}
