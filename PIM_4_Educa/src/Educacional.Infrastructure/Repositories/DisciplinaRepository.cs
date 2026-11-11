using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class DisciplinaRepository : IDisciplinaRepository
{
    private readonly EducacionalDbContext _db;

    public DisciplinaRepository(EducacionalDbContext db) => _db = db;

    public Task<List<DisciplinaDto>> ListarAsync() =>
        _db.Disciplinas
            .AsNoTracking()
            .OrderBy(d => d.Nome)
            .Select(d => new DisciplinaDto(d.Id, d.Nome, d.Descricao))
            .ToListAsync();

    public Task<bool> ExisteAsync(int id) => _db.Disciplinas.AnyAsync(d => d.Id == id);

    public Task<bool> NomeExisteAsync(string nome) => _db.Disciplinas.AnyAsync(d => d.Nome == nome);

    public async Task<DisciplinaDto> AdicionarAsync(Disciplina disciplina)
    {
        _db.Disciplinas.Add(disciplina);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2627 or 2601 })
        {
            throw new RegraDeNegocioException("Já existe uma disciplina com este nome.");
        }

        return new DisciplinaDto(disciplina.Id, disciplina.Nome, disciplina.Descricao);
    }
}
