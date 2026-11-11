using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public class HomeController : BaseController
{
    private readonly EducacionalApi _api;

    public HomeController(EducacionalApi api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var modelo = new DashboardViewModel
        {
            Nome = User.Identity?.Name ?? string.Empty,
            Perfil = Perfil
        };

        if (Perfil == Perfis.Admin)
        {
            modelo.TotalUsuarios = (await _api.UsuariosAsync()).Count;
            modelo.TotalDisciplinas = (await _api.DisciplinasAsync()).Count;
            modelo.TotalTurmas = (await _api.TodasTurmasAsync()).Count;
        }
        else
        {
            modelo.Turmas = await _api.MinhasTurmasAsync();
            if (Perfil == Perfis.Aluno)
                modelo.Pendentes = await _api.PendentesAsync();
        }

        return View(modelo);
    }

    [AllowAnonymous]
    public IActionResult Error() =>
        View("Erro", new ErroViewModel(500, "Ocorreu um erro inesperado. Tente novamente."));
}
