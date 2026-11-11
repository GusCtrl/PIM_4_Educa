using Educacional.Api.Extensions;
using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Route("api/atividades")]
public class AtividadesController : ControllerBase
{
    private readonly AtividadeService _atividades;
    private readonly EntregaService _entregas;

    public AtividadesController(AtividadeService atividades, EntregaService entregas)
    {
        _atividades = atividades;
        _entregas = entregas;
    }

    /// <summary>Professor cria uma atividade em uma das suas turmas.</summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Professor)]
    public async Task<ActionResult<AtividadeDto>> Criar(CriarAtividadeRequest request)
    {
        var criada = await _atividades.CriarAsync(request, User.ObterId());
        return StatusCode(StatusCodes.Status201Created, criada);
    }

    /// <summary>Aluno lista as atividades que ainda pode entregar (usa sp_AtividadesPendentes).</summary>
    [HttpGet("pendentes")]
    [Authorize(Roles = Perfis.Aluno)]
    public async Task<ActionResult<List<AtividadePendenteDto>>> Pendentes() =>
        Ok(await _atividades.ListarPendentesAsync(User.ObterId()));

    /// <summary>Detalhe da atividade (para o aluno, inclui a entrega que ele já enviou).</summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<AtividadeDetalheDto>> Detalhe(int id) =>
        Ok(await _atividades.ObterDetalheAsync(id, User.ObterId(), User.ObterPerfil()));

    /// <summary>Entregas de uma atividade, para correção (professor da turma ou Admin).</summary>
    [HttpGet("{id:int}/entregas")]
    [Authorize(Roles = Perfis.Professor + "," + Perfis.Admin)]
    public async Task<ActionResult<List<EntregaAlunoDto>>> Entregas(int id) =>
        Ok(await _entregas.ListarPorAtividadeAsync(id, User.ObterId(), User.ObterPerfil()));
}
