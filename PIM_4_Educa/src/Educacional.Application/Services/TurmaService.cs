using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain;

namespace Educacional.Application.Services;

public class TurmaService
{
    private readonly ITurmaRepository _turmas;
    private readonly IAtividadeRepository _atividades;

    public TurmaService(ITurmaRepository turmas, IAtividadeRepository atividades)
    {
        _turmas = turmas;
        _atividades = atividades;
    }

    /// <summary>Professor vê as turmas que leciona; aluno vê as turmas em que está matriculado.</summary>
    public Task<List<TurmaDto>> ListarMinhasAsync(int usuarioId, string perfil) =>
        perfil == Perfis.Professor
            ? _turmas.ListarDoProfessorAsync(usuarioId)
            : _turmas.ListarDoAlunoAsync(usuarioId);

    public async Task<TurmaDto> ObterAsync(int turmaId, int usuarioId, string perfil)
    {
        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil);
        return await _turmas.ObterDtoAsync(turmaId)
            ?? throw new NaoEncontradoException("Turma não encontrada.");
    }

    public async Task<List<AtividadeDto>> ListarAtividadesAsync(int turmaId, int usuarioId, string perfil)
    {
        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil);
        return await _atividades.ListarPorTurmaAsync(turmaId);
    }

    /// <summary>Alunos matriculados (somente professor da turma ou admin).</summary>
    public async Task<List<AlunoTurmaDto>> ListarAlunosAsync(int turmaId, int usuarioId, string perfil)
    {
        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil, somenteGestor: true);
        return await _turmas.ListarAlunosAsync(turmaId);
    }
}
