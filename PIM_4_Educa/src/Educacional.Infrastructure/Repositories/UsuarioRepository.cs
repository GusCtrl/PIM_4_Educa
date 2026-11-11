using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain.Entities;
using Educacional.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly EducacionalDbContext _db;

    public UsuarioRepository(EducacionalDbContext db) => _db = db;

    public Task<Usuario?> ObterPorEmailAsync(string email) =>
        _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public Task<Usuario?> ObterPorIdAsync(int id) =>
        _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public Task<List<UsuarioDto>> ListarAsync(string? perfil)
    {
        var consulta = _db.Usuarios.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(perfil))
            consulta = consulta.Where(u => u.Perfil == perfil);

        return consulta
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioDto(u.Id, u.Nome, u.Email, u.Perfil, u.Ativo))
            .ToListAsync();
    }

    public Task<bool> EmailExisteAsync(string email) =>
        _db.Usuarios.AnyAsync(u => u.Email == email);

    public async Task<UsuarioDto> AdicionarAsync(Usuario usuario)
    {
        _db.Usuarios.Add(usuario);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2627 or 2601 })
        {
            throw new RegraDeNegocioException("Já existe um usuário com este e-mail.");
        }

        return new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil, usuario.Ativo);
    }

    public async Task<bool> AlterarAtivoAsync(int id, bool ativo)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null) return false;

        usuario.Ativo = ativo;
        await _db.SaveChangesAsync();
        return true;
    }
}
