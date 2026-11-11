using Educacional.Application.Dtos;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class TurmaRepository : ITurmaRepository
{
    private readonly EducacionalDbContext _db;

    public TurmaRepository(EducacionalDbContext db) => _db = db;

    public Task<List<TurmaDto>> ListarTodasAsync() =>
        _db.Turmas
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .Select(t => new TurmaDto(t.Id, t.Nome, t.Disciplina.Nome, t.Periodo, t.Professor.Nome))
            .ToListAsync();

    public Task<List<TurmaDto>> ListarDoAlunoAsync(int alunoId) =>
        _db.Matriculas
            .AsNoTracking()
            .Where(m => m.AlunoId == alunoId)
            .OrderBy(m => m.Turma.Nome)
            .Select(m => new TurmaDto(
                m.Turma.Id,
                m.Turma.Nome,
                m.Turma.Disciplina.Nome,
                m.Turma.Periodo,
                m.Turma.Professor.Nome))
            .ToListAsync();

    public Task<List<TurmaDto>> ListarDoProfessorAsync(int professorId) =>
        _db.Turmas
            .AsNoTracking()
            .Where(t => t.ProfessorId == professorId)
            .OrderBy(t => t.Nome)
            .Select(t => new TurmaDto(t.Id, t.Nome, t.Disciplina.Nome, t.Periodo, t.Professor.Nome))
            .ToListAsync();

    public Task<TurmaDto?> ObterDtoAsync(int turmaId) =>
        _db.Turmas
            .AsNoTracking()
            .Where(t => t.Id == turmaId)
            .Select(t => new TurmaDto(t.Id, t.Nome, t.Disciplina.Nome, t.Periodo, t.Professor.Nome))
            .FirstOrDefaultAsync();

    public Task<Turma?> ObterPorIdAsync(int turmaId) =>
        _db.Turmas.AsNoTracking().FirstOrDefaultAsync(t => t.Id == turmaId);

    public async Task<TurmaDto> AdicionarAsync(Turma turma)
    {
        _db.Turmas.Add(turma);
        await _db.SaveChangesAsync();
        return (await ObterDtoAsync(turma.Id))!;
    }

    public Task<List<AlunoTurmaDto>> ListarAlunosAsync(int turmaId) =>
        _db.Matriculas
            .AsNoTracking()
            .Where(m => m.TurmaId == turmaId)
            .OrderBy(m => m.Aluno.Nome)
            .Select(m => new AlunoTurmaDto(m.AlunoId, m.Aluno.Nome, m.Aluno.Email))
            .ToListAsync();

    public Task<bool> AlunoMatriculadoAsync(int turmaId, int alunoId) =>
        _db.Matriculas.AnyAsync(m => m.TurmaId == turmaId && m.AlunoId == alunoId);

    public Task<bool> ProfessorLecionaAsync(int turmaId, int professorId) =>
        _db.Turmas.AnyAsync(t => t.Id == turmaId && t.ProfessorId == professorId);
}
