using Educacional.Application.Dtos;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly EducacionalDbContext _db;

    public MaterialRepository(EducacionalDbContext db) => _db = db;

    public Task<List<MaterialDto>> ListarPorDisciplinaAsync(int disciplinaId) =>
        _db.Materiais
            .AsNoTracking()
            .Where(m => m.DisciplinaId == disciplinaId)
            .OrderBy(m => m.Titulo)
            .Select(m => new MaterialDto(m.Id, m.DisciplinaId, m.Titulo, m.Url))
            .ToListAsync();

    public async Task<MaterialDto> AdicionarAsync(Material material)
    {
        _db.Materiais.Add(material);
        await _db.SaveChangesAsync();
        return new MaterialDto(material.Id, material.DisciplinaId, material.Titulo, material.Url);
    }
}
