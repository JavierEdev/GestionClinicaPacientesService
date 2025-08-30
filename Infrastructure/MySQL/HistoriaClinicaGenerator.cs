using pacientes_service.Domain.Interfaces;
using pacientes_service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace pacientes_service.Infrastructure.MySql;

public class HistoriaClinicaGenerator : IHistoriaClinicaGenerator
{
    private readonly IDbContextFactory<AppDbContext> _ctxFactory;

    public HistoriaClinicaGenerator(IDbContextFactory<AppDbContext> ctxFactory)
    {
        _ctxFactory = ctxFactory;
    }

    public async Task<string> NextAsync(int idPaciente, CancellationToken ct)
    {
        await using var db = _ctxFactory.CreateDbContext();
        var year = DateTime.UtcNow.Year;
        var next = await db.HistoriasClinicas.CountAsync(h => h.IdPaciente == idPaciente, ct) + 1;
        return $"HC-{year}-{idPaciente}-{next.ToString().PadLeft(6, '0')}";
    }
}
