using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Infrastructure.MySql;
using Microsoft.EntityFrameworkCore;

public class CreateConsultaMedicaService
{
    private readonly AppDbContext _db;
    public CreateConsultaMedicaService(AppDbContext db) => _db = db;

    public async Task<int> HandleAsync(CreateConsultaMedicaCommand cmd, CancellationToken ct)
    {
        var cita = await _db.CitasMedicas.AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCita == cmd.IdCita, ct);

        if (cita is null || cita.IdPaciente != cmd.IdPaciente || cita.IdMedico != cmd.IdMedico)
            throw new InvalidOperationException("La cita no es válida para generar la consulta.");

        var yaExiste = await _db.ConsultasMedicas.AsNoTracking()
            .AnyAsync(x => x.IdCita == cmd.IdCita, ct);
        if (yaExiste)
            throw new InvalidOperationException("Ya existe una consulta para esa cita.");

        var entity = new ConsultaMedica
        {
            IdPaciente = cmd.IdPaciente,
            IdMedico = cmd.IdMedico,
            Fecha = cmd.Fecha,
            MotivoConsulta = cmd.MotivoConsulta,
            Diagnostico = cmd.Diagnostico,
            Observaciones = cmd.Observaciones,
            IdCita = cmd.IdCita
        };

        _db.ConsultasMedicas.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.IdConsulta;
    }
}
