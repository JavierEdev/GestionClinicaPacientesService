namespace pacientes_service.Domain.Entities
{
    public class ProcedimientoMedico
    {
        public int IdProcedimiento { get; set; }
        public int IdConsulta { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = default!;
        public string? Descripcion { get; set; }
        public decimal? Costo { get; set; }
        public string Estado { get; set; } = "pendiente";
    }
}
