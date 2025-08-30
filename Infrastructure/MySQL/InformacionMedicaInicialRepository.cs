using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Infrastructure.MySQL 
{
    public class InformacionMedicaInicialRepository : IInformacionMedicaInicialRepository
    {
        private readonly AppDbContext _db;
        public InformacionMedicaInicialRepository(AppDbContext db) => _db = db;

        public async Task<AntecedenteMedico?> GetRegistroParaUpsertAsync(int idPaciente)
        {
            var query = _db.AntecedentesMedicos
                .Where(x => x.IdPaciente == idPaciente)
                .OrderByDescending(x => x.IdAntecedente);

            var conFicha = await query.FirstOrDefaultAsync(x =>
                x.Antecedentes != null || x.Alergias != null || x.EnfermedadesCronicas != null);

            if (conFicha != null) return conFicha;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<AntecedenteMedico> AddAsync(AntecedenteMedico entity)
        {
            _db.AntecedentesMedicos.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(AntecedenteMedico entity)
        {
            _db.AntecedentesMedicos.Update(entity);
            await _db.SaveChangesAsync();
        }
        public async Task<(IReadOnlyList<AntecedenteMedico> items, int total)> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 50;

            var query = _db.AntecedentesMedicos.AsNoTracking().OrderBy(a => a.IdAntecedente);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
