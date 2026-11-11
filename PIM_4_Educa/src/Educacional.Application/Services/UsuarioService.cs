using System.Net.Mail;
using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain;
using Educacional.Domain.Entities;

namespace Educacional.Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;

    public UsuarioService(IUsuarioRepository usuarios, IPasswordHasher hasher)
    {
        _usuarios = usuarios;
        _hasher = hasher;
    }

    public Task<List<UsuarioDto>> ListarAsync(string? perfil)
    {
        if (!string.IsNullOrWhiteSpace(perfil) && !Perfis.Todos.Contains(perfil))
            throw new RegraDeNegocioException("Perfil inválido.");

        return _usuarios.ListarAsync(string.IsNullOrWhiteSpace(perfil) ? null : perfil);
    }

    public async Task<UsuarioDto> CriarAsync(CriarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new RegraDeNegocioException("O nome é obrigatório.");

        var email = (request.Email ?? string.Empty).Trim();
        if (email.Length > 150 || !MailAddress.TryCreate(email, out _))
            throw new RegraDeNegocioException("Informe um e-mail válido.");

        if (string.IsNullOrEmpty(request.Senha) || request.Senha.Length < 6)
            throw new RegraDeNegocioException("A senha deve ter pelo menos 6 caracteres.");

        if (!Perfis.Todos.Contains(request.Perfil))
            throw new RegraDeNegocioException("Perfil inválido.");

        if (await _usuarios.EmailExisteAsync(email))
            throw new RegraDeNegocioException("Já existe um usuário com este e-mail.");

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = _hasher.Gerar(request.Senha),
            Perfil = request.Perfil,
            Ativo = true
        };

        return await _usuarios.AdicionarAsync(usuario);
    }

    public async Task AlterarAtivoAsync(int id, bool ativo, int adminLogadoId)
    {
        if (id == adminLogadoId && !ativo)
            throw new RegraDeNegocioException("Você não pode desativar a própria conta.");

        if (!await _usuarios.AlterarAtivoAsync(id, ativo))
            throw new NaoEncontradoException("Usuário não encontrado.");
    }
}
