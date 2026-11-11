using Educacional.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Educacional.Infrastructure.Data;

public static class SeedInicial
{
    public const string SenhaPadrao = "Senha@123";

    /// <summary>
    /// O script SQL cria os usuários com SenhaHash = 'TROCAR'. Na primeira execução
    /// a API gera o hash BCrypt real (senha de teste: Senha@123).
    /// </summary>
    public static async Task AjustarSenhasAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<EducacionalDbContext>();
        var hasher = services.GetRequiredService<IPasswordHasher>();

        var pendentes = await db.Usuarios.Where(u => u.SenhaHash == "TROCAR").ToListAsync();
        if (pendentes.Count == 0) return;

        foreach (var usuario in pendentes)
            usuario.SenhaHash = hasher.Gerar(SenhaPadrao);

        await db.SaveChangesAsync();
    }
}
