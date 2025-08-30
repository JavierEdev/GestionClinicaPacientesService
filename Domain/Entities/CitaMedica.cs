namespace pacientes_service.Domain.Entities
{
    public class CitaMedica
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = default!;
        public string? RazonCancelacion { get; set; }
    }
}
