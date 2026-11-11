using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain;
using Educacional.Domain.Entities;

namespace Educacional.Application.Services;

/// <summary>Cadastros administrativos: disciplinas, turmas e matrículas.</summary>
public class AdminService
{
    private readonly IDisciplinaRepository _disciplinas;
    private readonly ITurmaRepository _turmas;
    private readonly IMatriculaRepository _matriculas;
    private readonly IUsuarioRepository _usuarios;

    public AdminService(IDisciplinaRepository disciplinas, ITurmaRepository turmas,
                        IMatriculaRepository matriculas, IUsuarioRepository usuarios)
    {
        _disciplinas = disciplinas;
        _turmas = turmas;
        _matriculas = matriculas;
        _usuarios = usuarios;
    }

    // ---------- Disciplinas ----------
    public Task<List<DisciplinaDto>> ListarDisciplinasAsync() => _disciplinas.ListarAsync();

    public async Task<DisciplinaDto> CriarDisciplinaAsync(CriarDisciplinaRequest request)
    {
        var nome = (request.Nome ?? string.Empty).Trim();
        if (nome.Length == 0)
            throw new RegraDeNegocioException("O nome da disciplina é obrigatório.");
        if (nome.Length > 100)
            throw new RegraDeNegocioException("O nome da disciplina deve ter no máximo 100 caracteres.");
        if (await _disciplinas.NomeExisteAsync(nome))
            throw new RegraDeNegocioException("Já existe uma disciplina com este nome.");

        var descricao = request.Descricao?.Trim();
        if (descricao is { Length: > 300 })
            throw new RegraDeNegocioException("A descrição deve ter no máximo 300 caracteres.");

        return await _disciplinas.AdicionarAsync(new Disciplina { Nome = nome, Descricao = descricao });
    }

    // ---------- Turmas ----------
    public Task<List<TurmaDto>> ListarTurmasAsync() => _turmas.ListarTodasAsync();

    public async Task<TurmaDto> CriarTurmaAsync(CriarTurmaRequest request)
    {
        var nome = (request.Nome ?? string.Empty).Trim();
        var periodo = (request.Periodo ?? string.Empty).Trim();

        if (nome.Length == 0 || nome.Length > 60)
            throw new RegraDeNegocioException("Informe o nome da turma (até 60 caracteres).");
        if (periodo.Length == 0 || periodo.Length > 10)
            throw new RegraDeNegocioException("Informe o período (até 10 caracteres, ex.: 2026/2).");

        if (!await _disciplinas.ExisteAsync(request.DisciplinaId))
            throw new NaoEncontradoException("Disciplina não encontrada.");

        var professor = await _usuarios.ObterPorIdAsync(request.ProfessorId);
        if (professor is null || professor.Perfil != Perfis.Professor || !professor.Ativo)
            throw new RegraDeNegocioException("Selecione um professor ativo.");

        var turma = new Turma
        {
            DisciplinaId = request.DisciplinaId,
            ProfessorId = request.ProfessorId,
            Nome = nome,
            Periodo = periodo
        };

        return await _turmas.AdicionarAsync(turma);
    }

    // ---------- Matrículas ----------
    public async Task MatricularAsync(MatriculaRequest request)
    {
        if (await _turmas.ObterPorIdAsync(request.TurmaId) is null)
            throw new NaoEncontradoException("Turma não encontrada.");

        var aluno = await _usuarios.ObterPorIdAsync(request.AlunoId);
        if (aluno is null || aluno.Perfil != Perfis.Aluno)
            throw new RegraDeNegocioException("Selecione um aluno válido.");
        if (!aluno.Ativo)
            throw new RegraDeNegocioException("O aluno está inativo.");

        if (await _matriculas.ExisteAsync(request.TurmaId, request.AlunoId))
            throw new RegraDeNegocioException("O aluno já está matriculado nesta turma.");

        await _matriculas.AdicionarAsync(new Matricula { TurmaId = request.TurmaId, AlunoId = request.AlunoId });
    }

    public async Task RemoverMatriculaAsync(int turmaId, int alunoId)
    {
        if (!await _matriculas.RemoverAsync(turmaId, alunoId))
            throw new NaoEncontradoException("Matrícula não encontrada.");
    }
}
