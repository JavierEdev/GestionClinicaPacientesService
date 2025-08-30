namespace pacientes_service.Domain.Interfaces
{
    public interface ISecurePasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
