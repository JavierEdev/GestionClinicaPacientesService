using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces;

public interface IHistoriaClinicaRepository
{
    Task<HistoriaClinica?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(HistoriaClinica historia, CancellationToken ct);
}
