using pacientes_service.Domain.Interfaces;

namespace pacientes_service.Infrastructure.Security;

public class BcryptPasswordHasher : ISecurePasswordHasher
{
    private const int WorkFactor = 11;
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
