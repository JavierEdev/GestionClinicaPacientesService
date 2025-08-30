namespace pacientes_service.Communication.Commands
{
    public class CreateEmergencyContactCommand
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; } = null!;
        public string Parentesco { get; set; } = null!;
        public string Telefono { get; set; } = null!;
    }
}
