using Educacional.Application.Dtos;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class AvisoRepository : IAvisoRepository
{
    private readonly EducacionalDbContext _db;

    public AvisoRepository(EducacionalDbContext db) => _db = db;

    public Task<List<AvisoDto>> ListarPorTurmaAsync(int turmaId) =>
        _db.Avisos
            .AsNoTracking()
            .Where(a => a.TurmaId == turmaId)
            .OrderByDescending(a => a.DataPublicacao)
            .Select(a => new AvisoDto(a.Id, a.TurmaId, a.Autor.Nome, a.Texto, a.DataPublicacao))
            .ToListAsync();

    public async Task<AvisoDto> AdicionarAsync(Aviso aviso)
    {
        _db.Avisos.Add(aviso);
        await _db.SaveChangesAsync();

        var autor = await _db.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == aviso.AutorId)
            .Select(u => u.Nome)
            .FirstAsync();

        return new AvisoDto(aviso.Id, aviso.TurmaId, autor, aviso.Texto, aviso.DataPublicacao);
    }
}
