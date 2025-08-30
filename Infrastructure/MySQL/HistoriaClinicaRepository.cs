using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pacientes_service.Infrastructure.MySql;

public class HistoriaClinicaRepository : IHistoriaClinicaRepository
{
    private readonly IDbContextFactory<AppDbContext> _ctxFactory;
    public HistoriaClinicaRepository(IDbContextFactory<AppDbContext> ctxFactory)
    {
        _ctxFactory = ctxFactory;
    }

    public async Task<HistoriaClinica?> GetByIdAsync(int id, CancellationToken ct)
    {
        await using var db = _ctxFactory.CreateDbContext();
        return await db.HistoriasClinicas.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(HistoriaClinica historia, CancellationToken ct)
    {
        await using var db = _ctxFactory.CreateDbContext();
        db.HistoriasClinicas.Add(historia);
        await db.SaveChangesAsync(ct);
    }
}
