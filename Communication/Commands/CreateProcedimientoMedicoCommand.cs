namespace pacientes_service.Communication.Commands
{
    public class CreateProcedimientoMedicoCommand
    {
        public int IdConsulta { get; set; }
        public int IdProcedimientoCatalogo { get; set; }
    }
}
