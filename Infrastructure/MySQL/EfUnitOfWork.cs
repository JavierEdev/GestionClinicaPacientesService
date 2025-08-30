using pacientes_service.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pacientes_service.Infrastructure.MySql;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public EfUnitOfWork(AppDbContext db) => _db = db;

    public async Task BeginAsync(CancellationToken ct)
    {
        await _db.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        await _db.Database.CommitTransactionAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        await _db.Database.RollbackTransactionAsync(ct);
    }
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    => _db.SaveChangesAsync(ct);
}
