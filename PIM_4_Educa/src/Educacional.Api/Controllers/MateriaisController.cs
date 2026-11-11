using Educacional.Api.Extensions;
using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/turmas/{turmaId:int}/materiais")]
public class MateriaisController : ControllerBase
{
    private readonly MaterialService _materiais;

    public MateriaisController(MaterialService materiais) => _materiais = materiais;

    /// <summary>Materiais de apoio da disciplina da turma.</summary>
    [HttpGet]
    public async Task<ActionResult<List<MaterialDto>>> Listar(int turmaId) =>
        Ok(await _materiais.ListarAsync(turmaId, User.ObterId(), User.ObterPerfil()));

    /// <summary>Adiciona um material à disciplina da turma (professor da turma ou Admin).</summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Professor + "," + Perfis.Admin)]
    public async Task<ActionResult<MaterialDto>> Criar(int turmaId, CriarMaterialRequest request)
    {
        var material = await _materiais.CriarAsync(turmaId, request, User.ObterId(), User.ObterPerfil());
        return StatusCode(StatusCodes.Status201Created, material);
    }
}
