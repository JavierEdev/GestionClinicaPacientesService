using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Services
{
    public class CatalogoProcedimientoService
    {
        private readonly AppDbContext _context;

        public CatalogoProcedimientoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CatalogoProcedimiento>> ObtenerTodosCatalogosAsync()
        {
            return await _context.CatalogoProcedimientos
                .ToListAsync();
        }
    }
}
