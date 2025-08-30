namespace pacientes_service.Communication.Commands
{
    public class SoftDeleteUserCommand
    {
        public int Id { get; set; }
        public string? DeletedBy { get; set; }
    }
}
