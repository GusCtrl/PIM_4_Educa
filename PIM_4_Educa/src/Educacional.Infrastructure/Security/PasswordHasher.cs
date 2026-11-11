using Educacional.Application.Interfaces;

namespace Educacional.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Gerar(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);

    public bool Verificar(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch
        {
            // Hash inválido (ex.: 'TROCAR' ainda não substituído) nunca autentica.
            return false;
        }
    }
}
