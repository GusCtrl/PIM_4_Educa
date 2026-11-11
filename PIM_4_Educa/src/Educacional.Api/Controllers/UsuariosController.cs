using Educacional.Api.Extensions;
using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Authorize(Roles = Perfis.Admin)]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarios;

    public UsuariosController(UsuarioService usuarios) => _usuarios = usuarios;

    /// <summary>Lista usuários; filtre por perfil com ?perfil=Aluno|Professor|Admin.</summary>
    [HttpGet]
    public async Task<ActionResult<List<UsuarioDto>>> Listar([FromQuery] string? perfil) =>
        Ok(await _usuarios.ListarAsync(perfil));

    /// <summary>Cadastra um usuário.</summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Criar(CriarUsuarioRequest request) =>
        StatusCode(StatusCodes.Status201Created, await _usuarios.CriarAsync(request));

    /// <summary>Ativa ou desativa um usuário.</summary>
    [HttpPut("{id:int}/ativo")]
    public async Task<IActionResult> AlterarAtivo(int id, AlterarAtivoRequest request)
    {
        await _usuarios.AlterarAtivoAsync(id, request.Ativo, User.ObterId());
        return NoContent();
    }
}
