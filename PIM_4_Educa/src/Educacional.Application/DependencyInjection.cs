using Educacional.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Educacional.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<TurmaService>();
        services.AddScoped<AtividadeService>();
        services.AddScoped<EntregaService>();
        services.AddScoped<AvisoService>();
        services.AddScoped<MaterialService>();
        services.AddScoped<UsuarioService>();
        services.AddScoped<AdminService>();
        return services;
    }
}
