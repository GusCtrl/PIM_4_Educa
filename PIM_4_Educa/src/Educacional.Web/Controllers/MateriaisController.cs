using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

[Authorize(Roles = Perfis.Professor + "," + Perfis.Admin)]
public class MateriaisController : BaseController
{
    private readonly EducacionalApi _api;

    public MateriaisController(EducacionalApi api) => _api = api;

    [HttpPost]
    public async Task<IActionResult> Criar(MaterialFormViewModel modelo)
    {
        if (!ModeloInvalido())
            await TentarAsync(() => _api.CriarMaterialAsync(modelo.TurmaId, modelo.Titulo, modelo.Url),
                              "Material adicionado.");

        return RedirectToAction("Detalhes", "Turmas", new { id = modelo.TurmaId });
    }
}
