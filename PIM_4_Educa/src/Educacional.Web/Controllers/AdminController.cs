using Educacional.Application.Dtos;
using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Educacional.Web.Controllers;

[Authorize(Roles = Perfis.Admin)]
public class AdminController : BaseController
{
    private readonly EducacionalApi _api;

    public AdminController(EducacionalApi api) => _api = api;

    // ---------------- Usuários ----------------
    public async Task<IActionResult> Usuarios() => View(await _api.UsuariosAsync());

    [HttpPost]
    public async Task<IActionResult> CriarUsuario(NovoUsuarioViewModel modelo)
    {
        if (!ModeloInvalido())
            await TentarAsync(() => _api.CriarUsuarioAsync(
                new CriarUsuarioRequest(modelo.Nome, modelo.Email, modelo.Senha, modelo.Perfil)),
                "Usuário cadastrado.");

        return RedirectToAction(nameof(Usuarios));
    }

    [HttpPost]
    public async Task<IActionResult> AlternarUsuario(int id, bool ativo)
    {
        await TentarAsync(() => _api.AlterarAtivoAsync(id, ativo),
                          ativo ? "Usuário ativado." : "Usuário desativado.");
        return RedirectToAction(nameof(Usuarios));
    }

    // ---------------- Disciplinas ----------------
    public async Task<IActionResult> Disciplinas() => View(await _api.DisciplinasAsync());

    [HttpPost]
    public async Task<IActionResult> CriarDisciplina(NovaDisciplinaViewModel modelo)
    {
        if (!ModeloInvalido())
            await TentarAsync(() => _api.CriarDisciplinaAsync(
                new CriarDisciplinaRequest(modelo.Nome, modelo.Descricao)),
                "Disciplina cadastrada.");

        return RedirectToAction(nameof(Disciplinas));
    }

    // ---------------- Turmas ----------------
    public async Task<IActionResult> Turmas()
    {
        var modelo = new AdminTurmasViewModel
        {
            Turmas = await _api.TodasTurmasAsync(),
            Disciplinas = (await _api.DisciplinasAsync())
                .Select(d => new SelectListItem(d.Nome, d.Id.ToString())).ToList(),
            Professores = (await _api.UsuariosAsync(Perfis.Professor))
                .Where(p => p.Ativo)
                .Select(p => new SelectListItem(p.Nome, p.Id.ToString())).ToList()
        };
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> CriarTurma(NovaTurmaViewModel modelo)
    {
        if (!ModeloInvalido())
            await TentarAsync(() => _api.CriarTurmaAsync(new CriarTurmaRequest(
                modelo.DisciplinaId!.Value, modelo.ProfessorId!.Value, modelo.Nome, modelo.Periodo)),
                "Turma criada.");

        return RedirectToAction(nameof(Turmas));
    }

    // ---------------- Matrículas ----------------
    public async Task<IActionResult> Matriculas(int? turmaId)
    {
        var modelo = new AdminMatriculasViewModel { Turmas = await _api.TodasTurmasAsync() };

        if (turmaId is not null)
        {
            modelo.Turma = modelo.Turmas.FirstOrDefault(t => t.Id == turmaId);
            if (modelo.Turma is not null)
            {
                modelo.Matriculados = await _api.AlunosAsync(turmaId.Value);

                var jaMatriculados = modelo.Matriculados.Select(a => a.AlunoId).ToHashSet();
                modelo.AlunosDisponiveis = (await _api.UsuariosAsync(Perfis.Aluno))
                    .Where(a => a.Ativo && !jaMatriculados.Contains(a.Id))
                    .Select(a => new SelectListItem($"{a.Nome} ({a.Email})", a.Id.ToString()))
                    .ToList();
            }
        }

        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Matricular(int turmaId, int? alunoId)
    {
        if (alunoId is null)
            Erro("Escolha um aluno para matricular.");
        else
            await TentarAsync(() => _api.MatricularAsync(turmaId, alunoId.Value), "Aluno matriculado.");

        return RedirectToAction(nameof(Matriculas), new { turmaId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoverMatricula(int turmaId, int alunoId)
    {
        await TentarAsync(() => _api.RemoverMatriculaAsync(turmaId, alunoId), "Matrícula removida.");
        return RedirectToAction(nameof(Matriculas), new { turmaId });
    }
}
