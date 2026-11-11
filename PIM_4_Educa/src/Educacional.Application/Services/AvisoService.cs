using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;

namespace Educacional.Application.Services;

public class AvisoService
{
    private readonly IAvisoRepository _avisos;
    private readonly ITurmaRepository _turmas;

    public AvisoService(IAvisoRepository avisos, ITurmaRepository turmas)
    {
        _avisos = avisos;
        _turmas = turmas;
    }

    public async Task<List<AvisoDto>> ListarAsync(int turmaId, int usuarioId, string perfil)
    {
        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil);
        return await _avisos.ListarPorTurmaAsync(turmaId);
    }

    public async Task<AvisoDto> CriarAsync(int turmaId, CriarAvisoRequest request, int usuarioId, string perfil)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            throw new RegraDeNegocioException("O texto do aviso é obrigatório.");

        if (request.Texto.Trim().Length > 1000)
            throw new RegraDeNegocioException("O aviso deve ter no máximo 1000 caracteres.");

        if (await _turmas.ObterPorIdAsync(turmaId) is null)
            throw new NaoEncontradoException("Turma não encontrada.");

        await AcessoTurma.GarantirAsync(_turmas, turmaId, usuarioId, perfil, somenteGestor: true);

        var aviso = new Aviso { TurmaId = turmaId, AutorId = usuarioId, Texto = request.Texto.Trim() };
        return await _avisos.AdicionarAsync(aviso);
    }
}
