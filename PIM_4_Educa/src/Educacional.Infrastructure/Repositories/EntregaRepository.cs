using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class EntregaRepository : IEntregaRepository
{
    private readonly EducacionalDbContext _db;

    public EntregaRepository(EducacionalDbContext db) => _db = db;

    public Task<bool> ExisteAsync(int atividadeId, int alunoId) =>
        _db.Entregas.AnyAsync(e => e.AtividadeId == atividadeId && e.AlunoId == alunoId);

    public async Task<EntregaDto> AdicionarAsync(Entrega entrega)
    {
        _db.Entregas.Add(entrega);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
        {
            // 2627/2601 = chave única (entrega duplicada); >= 50000 = THROW dos triggers/procedures
            if (sql.Number is 2627 or 2601)
                throw new RegraDeNegocioException("Você já enviou esta atividade.");
            if (sql.Number >= 50000)
                throw new RegraDeNegocioException(sql.Message);
            throw;
        }

        return new EntregaDto(entrega.Id, entrega.AtividadeId, entrega.DataEnvio, entrega.Nota);
    }

    public Task<MinhaEntregaDto?> ObterMinhaAsync(int atividadeId, int alunoId) =>
        _db.Entregas
            .AsNoTracking()
            .Where(e => e.AtividadeId == atividadeId && e.AlunoId == alunoId)
            .Select(e => new MinhaEntregaDto(e.Id, e.Conteudo, e.DataEnvio, e.Nota))
            .FirstOrDefaultAsync();

    public Task<List<EntregaAlunoDto>> ListarPorAtividadeAsync(int atividadeId) =>
        _db.Entregas
            .AsNoTracking()
            .Where(e => e.AtividadeId == atividadeId)
            .OrderBy(e => e.Aluno.Nome)
            .Select(e => new EntregaAlunoDto(e.Id, e.AlunoId, e.Aluno.Nome, e.Conteudo, e.DataEnvio, e.Nota))
            .ToListAsync();

    // Chama a stored procedure dbo.sp_LancarNota (valida o professor e dispara o trigger de auditoria)
    public async Task LancarNotaAsync(int entregaId, decimal nota, int professorId)
    {
        try
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.sp_LancarNota {entregaId}, {nota}, {professorId}");
        }
        catch (SqlException ex) when (ex.Number >= 50000)
        {
            throw new RegraDeNegocioException(ex.Message);
        }
    }

    // Chama a stored procedure dbo.sp_BoletimAluno
    public Task<List<BoletimItemDto>> ObterBoletimAsync(int alunoId) =>
        _db.Database
            .SqlQuery<BoletimItemDto>($"EXEC dbo.sp_BoletimAluno {alunoId}")
            .ToListAsync();

    // Chama a stored procedure dbo.sp_MediaTurma
    public Task<List<MediaAlunoDto>> ObterMediaTurmaAsync(int turmaId) =>
        _db.Database
            .SqlQuery<MediaAlunoDto>($"EXEC dbo.sp_MediaTurma {turmaId}")
            .ToListAsync();
}
