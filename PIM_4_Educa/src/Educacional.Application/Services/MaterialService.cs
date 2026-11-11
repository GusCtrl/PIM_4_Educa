using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;

namespace Educacional.Application.Services;

public class MaterialService
{
    private readonly IMaterialRepository _materiais;
    private readonly ITurmaRepository _turmas;

    public MaterialService(IMaterialRepository materiais, ITurmaRepository turmas)
    {
        _materiais = materiais;
        _turmas = turmas;
    }

    /// <summary>Os materiais pertencem à disciplina; a turma só dá acesso a eles.</summary>
    public async Task<List<MaterialDto>> ListarAsync(int turmaId, int usuarioId, string perfil)
    {
        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil);

        var turma = await _turmas.ObterPorIdAsync(turmaId)
            ?? throw new NaoEncontradoException("Turma não encontrada.");

        return await _materiais.ListarPorDisciplinaAsync(turma.DisciplinaId);
    }

    public async Task<MaterialDto> CriarAsync(int turmaId, CriarMaterialRequest request, int usuarioId, string perfil)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo))
            throw new RegraDeNegocioException("O título é obrigatório.");

        if (request.Titulo.Trim().Length > 150)
            throw new RegraDeNegocioException("O título deve ter no máximo 150 caracteres.");

        // Só aceita links http/https: impede "javascript:" e similares de virarem links clicáveis.
        var url = (request.Url ?? string.Empty).Trim();
        if (url.Length > 400 ||
            !Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new RegraDeNegocioException("Informe um link válido, começando com http:// ou https://.");

        var turma = await _turmas.ObterPorIdAsync(turmaId)
            ?? throw new NaoEncontradoException("Turma não encontrada.");

        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil, somenteGestor: true);

        var material = new Material { DisciplinaId = turma.DisciplinaId, Titulo = request.Titulo.Trim(), Url = url };
        return await _materiais.AdicionarAsync(material);
    }
}
