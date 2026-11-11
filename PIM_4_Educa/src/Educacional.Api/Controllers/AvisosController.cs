using Educacional.Api.Extensions;
using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/turmas/{turmaId:int}/avisos")]
public class AvisosController : ControllerBase
{
    private readonly AvisoService _avisos;

    public AvisosController(AvisoService avisos) => _avisos = avisos;

    /// <summary>Mural de avisos da turma (mais recentes primeiro).</summary>
    [HttpGet]
    public async Task<ActionResult<List<AvisoDto>>> Listar(int turmaId) =>
        Ok(await _avisos.ListarAsync(turmaId, User.ObterId(), User.ObterPerfil()));

    /// <summary>Publica um aviso (professor da turma ou Admin).</summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Professor + "," + Perfis.Admin)]
    public async Task<ActionResult<AvisoDto>> Criar(int turmaId, CriarAvisoRequest request)
    {
        var aviso = await _avisos.CriarAsync(turmaId, request, User.ObterId(), User.ObterPerfil());
        return StatusCode(StatusCodes.Status201Created, aviso);
    }
}
