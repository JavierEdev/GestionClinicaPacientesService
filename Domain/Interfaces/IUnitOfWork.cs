namespace pacientes_service.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginAsync(CancellationToken ct);
        Task CommitAsync(CancellationToken ct);
        Task RollbackAsync(CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
