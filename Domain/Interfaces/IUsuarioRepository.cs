using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct);
        Task<IEnumerable<Usuario>> GetAllAsync(int? pacienteId, CancellationToken ct);
        Task<Usuario?> GetByIdAsync(int id, CancellationToken ct);
        Task<Usuario?> GetByNombreAsync(string username, CancellationToken ct);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct);
        Task AddAsync(Usuario user, CancellationToken ct);
        Task<bool> SoftDeleteAsync(int id, string? deletedBy, CancellationToken ct);
    }
}
