using System.Security.Claims;

namespace Educacional.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Id do usuário logado (claim gravada no token JWT).</summary>
    public static int ObterId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Perfil do usuário logado: Aluno, Professor ou Admin.</summary>
    public static string ObterPerfil(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
