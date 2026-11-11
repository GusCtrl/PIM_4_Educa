using Educacional.Application.Dtos;
using Educacional.Application.Services;
using Educacional.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Api.Controllers;

[ApiController]
[Authorize(Roles = Perfis.Admin)]
[Route("api/matriculas")]
public class MatriculasController : ControllerBase
{
    private readonly AdminService _admin;

    public MatriculasController(AdminService admin) => _admin = admin;

    /// <summary>Matricula um aluno em uma turma.</summary>
    [HttpPost]
    public async Task<IActionResult> Matricular(MatriculaRequest request)
    {
        await _admin.MatricularAsync(request);
        return NoContent();
    }

    /// <summary>Remove a matrícula: DELETE /api/matriculas?turmaId=1&amp;alunoId=4</summary>
    [HttpDelete]
    public async Task<IActionResult> Remover([FromQuery] int turmaId, [FromQuery] int alunoId)
    {
        await _admin.RemoverMatriculaAsync(turmaId, alunoId);
        return NoContent();
    }
}
