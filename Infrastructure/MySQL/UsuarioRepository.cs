using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Infrastructure.MySQL;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;
    public UsuarioRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Usuario>> GetAllAsync(int? pacienteId, CancellationToken ct)
    {
        var query = _db.Usuarios
            .AsNoTracking()
            .Where(u => !EF.Property<bool>(u, "eliminado"));

        if (pacienteId.HasValue)
            query = query.Where(u => u.IdPaciente == pacienteId.Value);

        return await query.ToListAsync(ct);
    }

    public Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
        => GetAllAsync(null, ct);

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct) =>
        _db.Usuarios.AsNoTracking()
           .FirstOrDefaultAsync(u => u.IdUsuario == id, ct);

    public Task<Usuario?> GetByNombreAsync(string username, CancellationToken ct) =>
        _db.Usuarios.AsNoTracking()
           .FirstOrDefaultAsync(u => u.NombreUsuario == username, ct);

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct) =>
        _db.Usuarios.AnyAsync(u => u.NombreUsuario == username, ct);

    public Task AddAsync(Usuario user, CancellationToken ct)
        => _db.Usuarios.AddAsync(user, ct).AsTask();

    public async Task<bool> SoftDeleteAsync(int id, string? deletedBy, CancellationToken ct)
    {
        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id, ct);
        if (user is null) return false;

        _db.Entry(user).Property("eliminado").CurrentValue = true;
        _db.Entry(user).Property("eliminado_en").CurrentValue = DateTime.UtcNow;
        _db.Entry(user).Property("eliminado_por").CurrentValue = deletedBy;

        return true; // UoW confirmará
    }
}
