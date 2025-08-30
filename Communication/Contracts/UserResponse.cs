namespace pacientes_service.Communication.Contracts
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public int? IdMedico { get; set; }
        public int? IdPaciente { get; set; }
    }
}
