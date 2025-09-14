using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces;

public interface IPacientesRepository
{
    Task<bool> ExistsByDpiAsync(string dpi, CancellationToken ct);
    Task<(int Id, string NumeroHC)> AddAsync(Paciente paciente, CancellationToken ct);
    Task<Paciente?> GetByIdAsync(int id, CancellationToken ct);
    Task<(IReadOnlyList<Paciente> items, int total)> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Paciente?> GetByDpiAsync(string dpi, CancellationToken ct = default);
}