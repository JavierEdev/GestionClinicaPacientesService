using pacientes_service.Domain.Entities;

namespace pacientes_service.Domain.Interfaces
{
    public interface IInformacionMedicaInicialRepository
    {
        Task<AntecedenteMedico?> GetRegistroParaUpsertAsync(int idPaciente);
        Task<AntecedenteMedico> AddAsync(AntecedenteMedico entity);
        Task UpdateAsync(AntecedenteMedico entity);
        Task<(IReadOnlyList<AntecedenteMedico> items, int total)> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
