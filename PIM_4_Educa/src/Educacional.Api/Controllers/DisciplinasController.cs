using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Authorize(Roles = Perfis.Admin)]
[Route("api/disciplinas")]
public class DisciplinasController : ControllerBase
{
    private readonly AdminService _admin;

    public DisciplinasController(AdminService admin) => _admin = admin;

    [HttpGet]
    public async Task<ActionResult<List<DisciplinaDto>>> Listar() =>
        Ok(await _admin.ListarDisciplinasAsync());

    [HttpPost]
    public async Task<ActionResult<DisciplinaDto>> Criar(CriarDisciplinaRequest request) =>
        StatusCode(StatusCodes.Status201Created, await _admin.CriarDisciplinaAsync(request));
}
