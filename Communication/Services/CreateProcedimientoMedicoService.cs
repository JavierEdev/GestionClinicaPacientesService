using Microsoft.EntityFrameworkCore;
using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Infrastructure.MySql;
using pacientes_service.Infrastructure.MySQL;

public class CreateProcedimientoMedicoService
{
    private readonly AppDbContext _db;
    public CreateProcedimientoMedicoService(AppDbContext db) => _db = db;

    public async Task<int> HandleAsync(CreateProcedimientoMedicoCommand cmd, CancellationToken ct)
    {
        var consulta = await _db.ConsultasMedicas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdConsulta == cmd.IdConsulta, ct);

        if (consulta is null || consulta.IdPaciente != cmd.IdPaciente)
            throw new InvalidOperationException("La consulta no existe o no corresponde al paciente.");

        if (consulta.IdMedico != cmd.IdMedico)
            throw new InvalidOperationException("El procedimiento debe ser registrado por el mismo médico de la consulta.");

        var entity = new ProcedimientoMedico
        {
            IdConsulta = cmd.IdConsulta,
            IdPaciente = cmd.IdPaciente,
            IdMedico = cmd.IdMedico,
            Fecha = cmd.Fecha,
            Tipo = cmd.Tipo.Trim(),
            Descripcion = cmd.Descripcion,
            Costo = cmd.Costo,
            Estado = "pendiente"
        };

        _db.ProcedimientosMedicos.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.IdProcedimiento;
    }
}
