using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class MatriculaRepository : IMatriculaRepository
{
    private readonly EducacionalDbContext _db;

    public MatriculaRepository(EducacionalDbContext db) => _db = db;

    public Task<bool> ExisteAsync(int turmaId, int alunoId) =>
        _db.Matriculas.AnyAsync(m => m.TurmaId == turmaId && m.AlunoId == alunoId);

    public async Task AdicionarAsync(Matricula matricula)
    {
        _db.Matriculas.Add(matricula);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
        {
            // 2627/2601 = matrícula duplicada; >= 50000 = THROW do trigger trg_Matricula_ValidaAluno
            if (sql.Number is 2627 or 2601)
                throw new RegraDeNegocioException("O aluno já está matriculado nesta turma.");
            if (sql.Number >= 50000)
                throw new RegraDeNegocioException(sql.Message);
            throw;
        }
    }

    public async Task<bool> RemoverAsync(int turmaId, int alunoId)
    {
        var matricula = await _db.Matriculas
            .FirstOrDefaultAsync(m => m.TurmaId == turmaId && m.AlunoId == alunoId);
        if (matricula is null) return false;

        _db.Matriculas.Remove(matricula);
        await _db.SaveChangesAsync();
        return true;
    }
}
