namespace pacientes_service.Communication.Commands
{
    public class CreateProcedimientoMedicoCommand
    {
        public int IdPaciente { get; set; }
        public int IdConsulta { get; set; }
        public int IdMedico { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = default!;
        public string? Descripcion { get; set; }
        public decimal? Costo { get; set; }
    }
}
