namespace pacientes_service.Domain.Interfaces;

public interface IInfrastructureFactory
{
    IPacientesRepository CreatePacientesRepository();
    IHistoriaClinicaRepository CreateHistoriaClinicaRepository();
    IContactoEmergenciaRepository CreateContactoEmergenciaRepository();
    IUnitOfWork CreateUnitOfWork();
    IHistoriaClinicaGenerator CreateHistoriaClinicaGenerator();
}
