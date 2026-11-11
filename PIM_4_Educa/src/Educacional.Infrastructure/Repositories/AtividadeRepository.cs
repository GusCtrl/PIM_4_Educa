using Educacional.Application.Dtos;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class AtividadeRepository : IAtividadeRepository
{
    private readonly EducacionalDbContext _db;

    public AtividadeRepository(EducacionalDbContext db) => _db = db;

    public Task<List<AtividadeDto>> ListarPorTurmaAsync(int turmaId) =>
        _db.Atividades
            .AsNoTracking()
            .Where(a => a.TurmaId == turmaId)
            .OrderBy(a => a.Prazo)
            .Select(a => new AtividadeDto(a.Id, a.TurmaId, a.Titulo, a.Descricao, a.Prazo))
            .ToListAsync();

    // Inclui a Turma porque o detalhe da atividade mostra o nome dela.
    public Task<Atividade?> ObterPorIdAsync(int id) =>
        _db.Atividades.AsNoTracking().Include(a => a.Turma).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<AtividadeDto> AdicionarAsync(Atividade atividade)
    {
        _db.Atividades.Add(atividade);
        await _db.SaveChangesAsync();
        return new AtividadeDto(atividade.Id, atividade.TurmaId, atividade.Titulo, atividade.Descricao, atividade.Prazo);
    }

    // Chama a stored procedure dbo.sp_AtividadesPendentes
    public Task<List<AtividadePendenteDto>> ListarPendentesAsync(int alunoId) =>
        _db.Database
            .SqlQuery<AtividadePendenteDto>($"EXEC dbo.sp_AtividadesPendentes {alunoId}")
            .ToListAsync();
}
