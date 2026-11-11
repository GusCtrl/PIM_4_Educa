using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

[Authorize(Roles = Perfis.Professor + "," + Perfis.Admin)]
public class AvisosController : BaseController
{
    private readonly EducacionalApi _api;

    public AvisosController(EducacionalApi api) => _api = api;

    [HttpPost]
    public async Task<IActionResult> Criar(AvisoFormViewModel modelo)
    {
        if (!ModeloInvalido())
            await TentarAsync(() => _api.CriarAvisoAsync(modelo.TurmaId, modelo.Texto), "Aviso publicado.");

        return RedirectToAction("Detalhes", "Turmas", new { id = modelo.TurmaId });
    }
}
