using Educacional.Api.Extensions;
using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Route("api/entregas")]
public class EntregasController : ControllerBase
{
    private readonly EntregaService _entregas;

    public EntregasController(EntregaService entregas) => _entregas = entregas;

    /// <summary>Aluno envia a entrega de uma atividade.</summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Aluno)]
    public async Task<ActionResult<EntregaDto>> Enviar(EnviarEntregaRequest request)
    {
        var entrega = await _entregas.EnviarAsync(request, User.ObterId());
        return StatusCode(StatusCodes.Status201Created, entrega);
    }

    /// <summary>Professor lança a nota de uma entrega (usa sp_LancarNota).</summary>
    [HttpPut("{id:int}/nota")]
    [Authorize(Roles = Perfis.Professor)]
    public async Task<IActionResult> LancarNota(int id, LancarNotaRequest request)
    {
        await _entregas.LancarNotaAsync(id, request.Nota, User.ObterId());
        return NoContent();
    }

    /// <summary>Boletim do aluno logado (usa sp_BoletimAluno).</summary>
    [HttpGet("/api/notas/boletim")]
    [Authorize(Roles = Perfis.Aluno)]
    public async Task<ActionResult<List<BoletimItemDto>>> Boletim() =>
        Ok(await _entregas.ObterBoletimAsync(User.ObterId()));
}
