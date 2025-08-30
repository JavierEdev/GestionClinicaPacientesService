using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Infrastructure.MySQL
{
    public class ContactoEmergenciaRepository : IContactoEmergenciaRepository
    {
        private readonly AppDbContext _db;
        public ContactoEmergenciaRepository(AppDbContext db) => _db = db;

        public async Task<List<ContactoEmergencia>> ListByPacienteAsync(int idPaciente, CancellationToken ct = default)
        {
            return await _db.ContactosEmergencia
                .AsNoTracking()
                .Where(c => c.IdPaciente == idPaciente)
                .OrderBy(c => c.IdContacto)
                .ToListAsync(ct);
        }

        public async Task<ContactoEmergencia?> GetByIdAsync(int idPaciente, int idContacto, CancellationToken ct = default)
        {
            return await _db.ContactosEmergencia
                .FirstOrDefaultAsync(c => c.IdContacto == idContacto && c.IdPaciente == idPaciente, ct);
        }

        public async Task<ContactoEmergencia> CreateAsync(ContactoEmergencia entity, CancellationToken ct = default)
        {
            await _db.ContactosEmergencia.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(ContactoEmergencia entity, CancellationToken ct = default)
        {
            _db.ContactosEmergencia.Update(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
