using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;

namespace pacientes_service.Communication.Services;

public class RegistrarPacienteService
{
    private readonly IInfrastructureFactory _factory;
    private readonly AppDbContext _db;

    public RegistrarPacienteService(IInfrastructureFactory factory, AppDbContext db)
    {
        _factory = factory;
        _db = db;
    }

    public async Task<(int Id, string HC)> HandleAsync(RegistrarPacienteCommand c, CancellationToken ct)
    {
        var repo = _factory.CreatePacientesRepository();
        var uow = _factory.CreateUnitOfWork();

        if (await repo.ExistsByDpiAsync(c.Dpi, ct))
            throw new InvalidOperationException("El DPI ya está registrado.");

        var paciente = new Paciente
        {
            Nombres = c.Nombres.Trim(),
            Apellidos = c.Apellidos.Trim(),
            Dpi = c.Dpi.Trim(),
            FechaNacimiento = c.FechaNacimiento,
            Sexo = c.Sexo,
            Direccion = c.Direccion,
            Telefono = c.Telefono,
            Correo = c.Correo,
            EstadoCivil = c.EstadoCivil
        };

        await uow.BeginAsync(ct);

        var resultado = await repo.AddAsync(paciente, ct);

        await uow.CommitAsync(ct);

        return resultado;
    }
}
