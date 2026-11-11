using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;

namespace Educacional.Application.Services;

public class EntregaService
{
    private readonly IAtividadeRepository _atividades;
    private readonly ITurmaRepository _turmas;
    private readonly IEntregaRepository _entregas;

    public EntregaService(IAtividadeRepository atividades, ITurmaRepository turmas, IEntregaRepository entregas)
    {
        _atividades = atividades;
        _turmas = turmas;
        _entregas = entregas;
    }

    public async Task<EntregaDto> EnviarAsync(EnviarEntregaRequest request, int alunoId)
    {
        if (string.IsNullOrWhiteSpace(request.Conteudo))
            throw new RegraDeNegocioException("O conteúdo da entrega é obrigatório.");

        var atividade = await _atividades.ObterPorIdAsync(request.AtividadeId)
            ?? throw new NaoEncontradoException("Atividade não encontrada.");

        if (!await _turmas.AlunoMatriculadoAsync(atividade.TurmaId, alunoId))
            throw new AcessoNegadoException("Você não está matriculado nesta turma.");

        // O trigger trg_Entrega_ValidaPrazo também protege o banco; aqui damos uma resposta amigável antes.
        if (DateTime.Now > atividade.Prazo)
            throw new RegraDeNegocioException("O prazo desta atividade já encerrou.");

        if (await _entregas.ExisteAsync(atividade.Id, alunoId))
            throw new RegraDeNegocioException("Você já enviou esta atividade.");

        var entrega = new Entrega
        {
            AtividadeId = atividade.Id,
            AlunoId = alunoId,
            Conteudo = request.Conteudo.Trim()
        };

        return await _entregas.AdicionarAsync(entrega);
    }

    /// <summary>Entregas de uma atividade para correção (professor da turma ou admin).</summary>
    public async Task<List<EntregaAlunoDto>> ListarPorAtividadeAsync(int atividadeId, int usuarioId, string perfil)
    {
        var atividade = await _atividades.ObterPorIdAsync(atividadeId)
            ?? throw new NaoEncontradoException("Atividade não encontrada.");

        await AcessoTurma.GarantirAsync(_turmas, atividade.TurmaId, usuarioId, perfil, somenteGestor: true);
        return await _entregas.ListarPorAtividadeAsync(atividadeId);
    }

    public Task LancarNotaAsync(int entregaId, decimal nota, int professorId)
    {
        if (nota < 0 || nota > 10)
            throw new RegraDeNegocioException("A nota deve estar entre 0 e 10.");

        return _entregas.LancarNotaAsync(entregaId, nota, professorId);
    }

    public Task<List<BoletimItemDto>> ObterBoletimAsync(int alunoId) =>
        _entregas.ObterBoletimAsync(alunoId);

    /// <summary>Média de cada aluno na turma (professor da turma ou admin).</summary>
    public async Task<List<MediaAlunoDto>> ObterMediaTurmaAsync(int turmaId, int usuarioId, string perfil)
    {
        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil, somenteGestor: true);
        return await _entregas.ObterMediaTurmaAsync(turmaId);
    }
}
