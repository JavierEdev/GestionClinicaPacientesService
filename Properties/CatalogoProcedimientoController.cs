using Microsoft.AspNetCore.Mvc;
using pacientes_service.Services;
using pacientes_service.Domain.Entities;

namespace pacientes_service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogoProcedimientoController : ControllerBase
    {
        private readonly CatalogoProcedimientoService _catalogoProcedimientoService;

        public CatalogoProcedimientoController(CatalogoProcedimientoService catalogoProcedimientoService)
        {
            _catalogoProcedimientoService = catalogoProcedimientoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CatalogoProcedimiento>>> Get()
        {
            var catalogoProcedimientos = await _catalogoProcedimientoService.ObtenerTodosCatalogosAsync();
            return Ok(catalogoProcedimientos);
        }
    }
}
