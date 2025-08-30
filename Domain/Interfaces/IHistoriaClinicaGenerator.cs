namespace pacientes_service.Domain.Interfaces;

public interface IHistoriaClinicaGenerator
{
    Task<string> NextAsync(int idPaciente, CancellationToken ct);
}
