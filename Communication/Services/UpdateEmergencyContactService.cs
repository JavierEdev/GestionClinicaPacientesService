using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Communication.Services
{
    public class UpdateEmergencyContactService
    {
        private readonly IContactoEmergenciaRepository _repo;

        public UpdateEmergencyContactService(IContactoEmergenciaRepository repo) { _repo = repo; }

        public async Task HandleAsync(UpdateEmergencyContactCommand cmd, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(cmd.IdPaciente, cmd.IdContacto, ct);
            if (entity is null)
                throw new ArgumentException("Contacto no encontrado para el paciente.");

            if (!string.IsNullOrWhiteSpace(cmd.Nombre)) entity.Nombre = cmd.Nombre.Trim();
            if (!string.IsNullOrWhiteSpace(cmd.Parentesco)) entity.Parentesco = cmd.Parentesco.Trim();
            if (!string.IsNullOrWhiteSpace(cmd.Telefono)) entity.Telefono = cmd.Telefono.Trim();

            await _repo.UpdateAsync(entity, ct);
        }
    }
}
