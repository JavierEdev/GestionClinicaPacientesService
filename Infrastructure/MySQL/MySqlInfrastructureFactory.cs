using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySQL;

namespace pacientes_service.Infrastructure.MySql;

public class MySqlInfrastructureFactory : IInfrastructureFactory
{
    private readonly IDbContextFactory<AppDbContext> _ctxFactory;
    private readonly IHistoriaClinicaGenerator _hcGenerator;

    public MySqlInfrastructureFactory(IDbContextFactory<AppDbContext> ctxFactory, IHistoriaClinicaGenerator hcGenerator)
    {
        _ctxFactory = ctxFactory;
        _hcGenerator = hcGenerator;
    }

    public IPacientesRepository CreatePacientesRepository()
    {
        var dbContext = _ctxFactory.CreateDbContext();
        return new PacientesRepository(dbContext, _hcGenerator);
    }

    public IHistoriaClinicaRepository CreateHistoriaClinicaRepository()
    {
        return new HistoriaClinicaRepository(_ctxFactory);
    }

    public IContactoEmergenciaRepository CreateContactoEmergenciaRepository()
    {
        return new ContactoEmergenciaRepository(_ctxFactory.CreateDbContext());
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        var dbContext = _ctxFactory.CreateDbContext();
        return new EfUnitOfWork(dbContext);
    }
    public IHistoriaClinicaGenerator CreateHistoriaClinicaGenerator()
    {
        return new HistoriaClinicaGenerator(_ctxFactory);
    }
}
