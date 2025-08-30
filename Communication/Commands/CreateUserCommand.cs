namespace pacientes_service.Communication.Commands
{
    public class CreateUserCommand
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public int? IdMedico { get; set; }
        public int? IdPaciente { get; set; }
    }
}
