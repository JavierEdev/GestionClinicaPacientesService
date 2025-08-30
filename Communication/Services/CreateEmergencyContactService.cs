using Microsoft.EntityFrameworkCore;
using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Communication.Services
{
    public class CreateEmergencyContactService
    {
        private readonly AppDbContext _db;
        private readonly IContactoEmergenciaRepository _repo;

        public CreateEmergencyContactService(AppDbContext db, IContactoEmergenciaRepository repo)
        { _db = db; _repo = repo; }

        public async Task<ContactoEmergencia> HandleAsync(CreateEmergencyContactCommand cmd, CancellationToken ct = default)
        {
            var exists = await _db.Pacientes
                .AsNoTracking()
                .Where(p => p.IdPaciente == cmd.IdPaciente)
                .Select(_ => 1)
                .FirstOrDefaultAsync(ct) == 1;
            if (!exists) throw new ArgumentException("Paciente no existe.");

            var entity = new ContactoEmergencia
            {
                IdPaciente = cmd.IdPaciente,
                Nombre = cmd.Nombre.Trim(),
                Parentesco = cmd.Parentesco.Trim(),
                Telefono = cmd.Telefono.Trim()
            };

            return await _repo.CreateAsync(entity, ct);
        }
    }
}
