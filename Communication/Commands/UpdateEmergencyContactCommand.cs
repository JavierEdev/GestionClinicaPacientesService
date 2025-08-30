namespace pacientes_service.Communication.Commands
{
    public class UpdateEmergencyContactCommand
    {
        public int IdPaciente { get; set; }
        public int IdContacto { get; set; }
        public string? Nombre { get; set; }
        public string? Parentesco { get; set; }
        public string? Telefono { get; set; }
    }
}
