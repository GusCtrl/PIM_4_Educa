using Educacional.Application.Dtos;
using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public class AtividadesController : BaseController
{
    private readonly EducacionalApi _api;

    public AtividadesController(EducacionalApi api) => _api = api;

    [Authorize(Roles = Perfis.Aluno)]
    public async Task<IActionResult> Pendentes() => View(await _api.PendentesAsync());

    public async Task<IActionResult> Detalhes(int id)
    {
        var modelo = new AtividadeDetalheViewModel { Atividade = await _api.ObterAtividadeAsync(id) };

        if (Perfil is Perfis.Professor or Perfis.Admin)
            modelo.Entregas = await _api.EntregasDaAtividadeAsync(id);

        return View(modelo);
    }

    [Authorize(Roles = Perfis.Professor)]
    [HttpGet]
    public async Task<IActionResult> Criar(int turmaId)
    {
        var turma = await _api.ObterTurmaAsync(turmaId);
        return View(new CriarAtividadeViewModel { TurmaId = turmaId, TurmaNome = turma.Nome });
    }

    [Authorize(Roles = Perfis.Professor)]
    [HttpPost]
    public async Task<IActionResult> Criar(CriarAtividadeViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        try
        {
            await _api.CriarAtividadeAsync(new CriarAtividadeRequest(
                modelo.TurmaId, modelo.Titulo, modelo.Descricao, modelo.Prazo!.Value));
        }
        catch (ApiException ex) when (ex.StatusCode is 400 or 403 or 404)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }

        Sucesso("Atividade criada.");
        return RedirectToAction("Detalhes", "Turmas", new { id = modelo.TurmaId });
    }
}
