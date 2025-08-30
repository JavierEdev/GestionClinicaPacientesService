namespace pacientes_service.Communication.Commands
{
    public class RefreshRequest
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
