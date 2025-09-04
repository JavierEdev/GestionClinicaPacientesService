using Microsoft.EntityFrameworkCore;
using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Infrastructure.MySql;

public class CreateProcedimientoMedicoService
{
    private readonly AppDbContext _db;

    public CreateProcedimientoMedicoService(AppDbContext db) => _db = db;

    public async Task<(int idProcedimiento, int idConsulta)> HandleAsync(CreateProcedimientoMedicoCommand cmd, CancellationToken ct)
    {
        // Verificar que la consulta exista
        var consulta = await _db.ConsultasMedicas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdConsulta == cmd.IdConsulta, ct);

        if (consulta == null)
            throw new InvalidOperationException("La consulta no existe.");

        // Verificar que el catálogo de procedimiento exista
        var catalogoProcedimiento = await _db.CatalogoProcedimientos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdProcedimientoCatalogo == cmd.IdProcedimientoCatalogo, ct);

        if (catalogoProcedimiento == null)
            throw new InvalidOperationException("El procedimiento del catálogo no existe.");

        // Crear el procedimiento
        var entity = new ProcedimientoMedico
        {
            IdConsulta = cmd.IdConsulta,
            IdProcedimientoCatalogo = cmd.IdProcedimientoCatalogo,
            Procedimiento = catalogoProcedimiento.Nombre,  // Usamos el nombre del catálogo
            Descripcion = catalogoProcedimiento.Descripcion // Usamos la descripción del catálogo
        };

        try
        {
            // Insertar el procedimiento en la base de datos
            _db.ProcedimientosMedicos.Add(entity);
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            // Log y manejo de error
            throw new InvalidOperationException("Ocurrió un error al intentar guardar el procedimiento médico.", ex);
        }

        return (entity.IdProcedimiento, entity.IdConsulta);
    }
}
