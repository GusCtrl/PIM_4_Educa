using Educacional.Application.Dtos;
using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;

namespace Educacional.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IUsuarioRepository usuarios, IPasswordHasher hasher, ITokenService tokens)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var email = (request.Email ?? string.Empty).Trim();
        var usuario = await _usuarios.ObterPorEmailAsync(email);

        // Mesma mensagem para "não existe" e "senha errada": não revela qual e-mail está cadastrado.
        if (usuario is null || !usuario.Ativo || !_hasher.Verificar(request.Senha ?? string.Empty, usuario.SenhaHash))
            throw new CredenciaisInvalidasException();

        return new LoginResponse(_tokens.GerarToken(usuario), usuario.Id, usuario.Nome, usuario.Perfil);
    }
}
