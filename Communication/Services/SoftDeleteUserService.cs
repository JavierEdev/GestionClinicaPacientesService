using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Communication.Services;

public class SoftDeleteUserService
{
    private readonly IUsuarioRepository _repo;
    private readonly IUnitOfWork _uow;
    public SoftDeleteUserService(IUsuarioRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<bool> ExecuteAsync(SoftDeleteUserCommand cmd, CancellationToken ct)
    {
        var ok = await _repo.SoftDeleteAsync(cmd.Id, cmd.DeletedBy, ct);
        if (!ok) return false;
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
