using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Infrastructure.MySql
{
    public class PacientesRepository : IPacientesRepository
    {
        private readonly AppDbContext _db;
        private readonly IHistoriaClinicaGenerator _hcGenerator;

        public PacientesRepository(AppDbContext db, IHistoriaClinicaGenerator hcGenerator)
        {
            _db = db;
            _hcGenerator = hcGenerator;
        }

        public async Task<bool> ExistsByDpiAsync(string dpi, CancellationToken ct)
        {
            return await _db.Pacientes.AnyAsync(p => p.Dpi == dpi, ct);
        }

        public async Task<(int Id, string NumeroHC)> AddAsync(Paciente paciente, CancellationToken ct)
        {
            var historiaExistente = await _db.HistoriasClinicas
                .FirstOrDefaultAsync(h => h.IdPaciente == paciente.IdPaciente, ct);

            if (historiaExistente != null)
            {
                return (paciente.IdPaciente, historiaExistente.NumeroHistoriaClinica);
            }
            var historiaClinica = new HistoriaClinica
            {
                IdPaciente = paciente.IdPaciente,
                NumeroHistoriaClinica = await _hcGenerator.NextAsync(paciente.IdPaciente, ct),
                Descripcion = "Consulta inicial"
            };

            paciente.HistoriasClinicas.Add(historiaClinica);

            _db.Pacientes.Add(paciente);

            await _db.SaveChangesAsync(ct);

            return (paciente.IdPaciente, historiaClinica.NumeroHistoriaClinica);
        }

        public async Task<Paciente?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.Pacientes
                .Include(p => p.HistoriasClinicas)
                .Include(p => p.ContactosEmergencia)
                .FirstOrDefaultAsync(p => p.IdPaciente == id, ct);
        }
        public async Task<(IReadOnlyList<Paciente> items, int total)> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 50;

            var query = _db.Pacientes.AsNoTracking().OrderBy(p => p.IdPaciente);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
