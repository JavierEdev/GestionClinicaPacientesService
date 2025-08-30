using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces
{
    public interface IContactoEmergenciaRepository
    {
        Task<List<ContactoEmergencia>> ListByPacienteAsync(int idPaciente, CancellationToken ct = default);
        Task<ContactoEmergencia?> GetByIdAsync(int idPaciente, int idContacto, CancellationToken ct = default);
        Task<ContactoEmergencia> CreateAsync(ContactoEmergencia entity, CancellationToken ct = default);
        Task UpdateAsync(ContactoEmergencia entity, CancellationToken ct = default);
    }
}
