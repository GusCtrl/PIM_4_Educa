using Educacional.Api.Extensions;
using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/turmas")]
public class TurmasController : ControllerBase
{
    private const string Gestores = Perfis.Professor + "," + Perfis.Admin;

    private readonly TurmaService _turmas;
    private readonly AdminService _admin;
    private readonly EntregaService _entregas;

    public TurmasController(TurmaService turmas, AdminService admin, EntregaService entregas)
    {
        _turmas = turmas;
        _admin = admin;
        _entregas = entregas;
    }

    /// <summary>Todas as turmas (somente Admin).</summary>
    [HttpGet]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<ActionResult<List<TurmaDto>>> Todas() =>
        Ok(await _admin.ListarTurmasAsync());

    /// <summary>Cria uma turma (somente Admin).</summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<ActionResult<TurmaDto>> Criar(CriarTurmaRequest request) =>
        StatusCode(StatusCodes.Status201Created, await _admin.CriarTurmaAsync(request));

    /// <summary>Turmas do usuário logado (aluno: matriculadas; professor: que leciona).</summary>
    [HttpGet("minhas")]
    public async Task<ActionResult<List<TurmaDto>>> Minhas() =>
        Ok(await _turmas.ListarMinhasAsync(User.ObterId(), User.ObterPerfil()));

    /// <summary>Dados de uma turma (somente quem participa dela).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TurmaDto>> Obter(int id) =>
        Ok(await _turmas.ObterAsync(id, User.ObterId(), User.ObterPerfil()));

    /// <summary>Atividades de uma turma (somente quem participa dela).</summary>
    [HttpGet("{id:int}/atividades")]
    public async Task<ActionResult<List<AtividadeDto>>> Atividades(int id) =>
        Ok(await _turmas.ListarAtividadesAsync(id, User.ObterId(), User.ObterPerfil()));

    /// <summary>Alunos matriculados na turma (professor da turma ou Admin).</summary>
    [HttpGet("{id:int}/alunos")]
    [Authorize(Roles = Gestores)]
    public async Task<ActionResult<List<AlunoTurmaDto>>> Alunos(int id) =>
        Ok(await _turmas.ListarAlunosAsync(id, User.ObterId(), User.ObterPerfil()));

    /// <summary>Média de cada aluno na turma (usa sp_MediaTurma).</summary>
    [HttpGet("{id:int}/media")]
    [Authorize(Roles = Gestores)]
    public async Task<ActionResult<List<MediaAlunoDto>>> Media(int id) =>
        Ok(await _entregas.ObterMediaTurmaAsync(id, User.ObterId(), User.ObterPerfil()));
}
