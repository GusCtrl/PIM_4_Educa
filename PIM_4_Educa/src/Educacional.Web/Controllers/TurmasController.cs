using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public class TurmasController : BaseController
{
    private readonly EducacionalApi _api;

    public TurmasController(EducacionalApi api) => _api = api;

    public async Task<IActionResult> Index() =>
        View(Perfil == Perfis.Admin
            ? await _api.TodasTurmasAsync()
            : await _api.MinhasTurmasAsync());

    public async Task<IActionResult> Detalhes(int id)
    {
        var gestor = Perfil is Perfis.Professor or Perfis.Admin;

        var modelo = new TurmaDetalheViewModel
        {
            Turma = await _api.ObterTurmaAsync(id),   // a API valida se o usuário participa da turma
            Atividades = await _api.AtividadesAsync(id),
            Avisos = await _api.AvisosAsync(id),
            Materiais = await _api.MateriaisAsync(id),
            PodeGerir = gestor
        };

        if (gestor)
            modelo.Alunos = await _api.AlunosAsync(id);

        return View(modelo);
    }
}
