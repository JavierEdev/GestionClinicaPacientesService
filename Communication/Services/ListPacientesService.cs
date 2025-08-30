using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Communication.Services
{
    public class ListPacientesService
    {
        private readonly IPacientesRepository _repo;
        public ListPacientesService(IPacientesRepository repo) => _repo = repo;

        public async Task<object> HandleAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var (items, total) = await _repo.GetAllAsync(page, pageSize, ct);
            return new { page, pageSize, total, items };
        }
    }
}
