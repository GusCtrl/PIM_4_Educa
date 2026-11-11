using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain;
using Educacional.Domain.Entities;

namespace Educacional.Application.Services;

public class AtividadeService
{
    private readonly IAtividadeRepository _atividades;
    private readonly ITurmaRepository _turmas;
    private readonly IEntregaRepository _entregas;

    public AtividadeService(IAtividadeRepository atividades, ITurmaRepository turmas, IEntregaRepository entregas)
    {
        _atividades = atividades;
        _turmas = turmas;
        _entregas = entregas;
    }

    public async Task<AtividadeDto> CriarAsync(CriarAtividadeRequest request, int professorId)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo))
            throw new RegraDeNegocioException("O título é obrigatório.");

        if (request.Titulo.Trim().Length > 150)
            throw new RegraDeNegocioException("O título deve ter no máximo 150 caracteres.");

        if (request.Prazo <= DateTime.Now)
            throw new RegraDeNegocioException("O prazo deve ser uma data futura.");

        if (!await _turmas.ProfessorLecionaAsync(request.TurmaId, professorId))
            throw new AcessoNegadoException("Você só pode criar atividades nas suas turmas.");

        var atividade = new Atividade
        {
            TurmaId = request.TurmaId,
            Titulo = request.Titulo.Trim(),
            Descricao = request.Descricao?.Trim(),
            Prazo = request.Prazo
        };

        return await _atividades.AdicionarAsync(atividade);
    }

    /// <summary>Detalhe da atividade; para o aluno inclui a entrega que ele já enviou (se houver).</summary>
    public async Task<AtividadeDetalheDto> ObterDetalheAsync(int atividadeId, int usuarioId, string perfil)
    {
        var atividade = await _atividades.ObterPorIdAsync(atividadeId)
            ?? throw new NaoEncontradoException("Atividade não encontrada.");

        await AcessoTurma.GarantirAsync(_turmas, atividade.TurmaId, usuarioId, perfil);

        MinhaEntregaDto? minha = perfil == Perfis.Aluno
            ? await _entregas.ObterMinhaAsync(atividade.Id, usuarioId)
            : null;

        return new AtividadeDetalheDto(atividade.Id, atividade.TurmaId, atividade.Turma.Nome,
                                       atividade.Titulo, atividade.Descricao, atividade.Prazo, minha);
    }

    public Task<List<AtividadePendenteDto>> ListarPendentesAsync(int alunoId) =>
        _atividades.ListarPendentesAsync(alunoId);
}
