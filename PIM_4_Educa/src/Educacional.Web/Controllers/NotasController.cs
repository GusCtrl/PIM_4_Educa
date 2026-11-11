using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public class NotasController : BaseController
{
    private readonly EducacionalApi _api;

    public NotasController(EducacionalApi api) => _api = api;

    [Authorize(Roles = Perfis.Aluno)]
    public async Task<IActionResult> Boletim() => View(await _api.BoletimAsync());

    [Authorize(Roles = Perfis.Professor + "," + Perfis.Admin)]
    public async Task<IActionResult> MediaTurma(int turmaId)
    {
        var modelo = new MediaTurmaViewModel
        {
            Turma = await _api.ObterTurmaAsync(turmaId),
            Alunos = await _api.MediaTurmaAsync(turmaId)
        };
        return View(modelo);
    }
}
