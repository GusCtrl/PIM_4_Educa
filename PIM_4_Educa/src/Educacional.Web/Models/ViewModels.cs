using System.ComponentModel.DataAnnotations;
using Educacional.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Educacional.Web.Models;

public record ErroViewModel(int Status, string Mensagem);

// ---------- Login ----------
public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

// ---------- Páginas ----------
public class DashboardViewModel
{
    public string Nome { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public List<TurmaDto> Turmas { get; set; } = new();
    public List<AtividadePendenteDto> Pendentes { get; set; } = new();
    public int TotalUsuarios { get; set; }
    public int TotalDisciplinas { get; set; }
    public int TotalTurmas { get; set; }
}

public class TurmaDetalheViewModel
{
    public TurmaDto Turma { get; set; } = null!;
    public List<AtividadeDto> Atividades { get; set; } = new();
    public List<AvisoDto> Avisos { get; set; } = new();
    public List<MaterialDto> Materiais { get; set; } = new();
    public List<AlunoTurmaDto> Alunos { get; set; } = new();
    public bool PodeGerir { get; set; }
}

public class AtividadeDetalheViewModel
{
    public AtividadeDetalheDto Atividade { get; set; } = null!;
    public List<EntregaAlunoDto> Entregas { get; set; } = new();
    public bool PrazoEncerrado => Atividade.Prazo < DateTime.Now;
}

public class MediaTurmaViewModel
{
    public TurmaDto Turma { get; set; } = null!;
    public List<MediaAlunoDto> Alunos { get; set; } = new();
}

public class AdminTurmasViewModel
{
    public List<TurmaDto> Turmas { get; set; } = new();
    public List<SelectListItem> Disciplinas { get; set; } = new();
    public List<SelectListItem> Professores { get; set; } = new();
}

public class AdminMatriculasViewModel
{
    public List<TurmaDto> Turmas { get; set; } = new();
    public TurmaDto? Turma { get; set; }
    public List<AlunoTurmaDto> Matriculados { get; set; } = new();
    public List<SelectListItem> AlunosDisponiveis { get; set; } = new();
}

// ---------- Formulários ----------
public class CriarAtividadeViewModel
{
    [Required]
    public int TurmaId { get; set; }

    [Required(ErrorMessage = "Informe o título.")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Informe o prazo de entrega.")]
    public DateTime? Prazo { get; set; }

    public string? TurmaNome { get; set; }
}

public class EnviarEntregaViewModel
{
    [Required] public int AtividadeId { get; set; }

    [Required(ErrorMessage = "Escreva a resposta da atividade antes de enviar.")]
    public string Conteudo { get; set; } = string.Empty;
}

public class AvisoFormViewModel
{
    [Required] public int TurmaId { get; set; }

    [Required(ErrorMessage = "Escreva o texto do aviso.")]
    [StringLength(1000, ErrorMessage = "O aviso deve ter no máximo 1000 caracteres.")]
    public string Texto { get; set; } = string.Empty;
}

public class MaterialFormViewModel
{
    [Required] public int TurmaId { get; set; }

    [Required(ErrorMessage = "Informe o título do material.")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o link do material.")]
    [StringLength(400, ErrorMessage = "O link deve ter no máximo 400 caracteres.")]
    public string Url { get; set; } = string.Empty;
}

public class NovoUsuarioViewModel
{
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escolha o perfil.")]
    public string Perfil { get; set; } = string.Empty;
}

public class NovaDisciplinaViewModel
{
    [Required(ErrorMessage = "Informe o nome da disciplina.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
    public string? Descricao { get; set; }
}

public class NovaTurmaViewModel
{
    [Required(ErrorMessage = "Escolha a disciplina.")]
    public int? DisciplinaId { get; set; }

    [Required(ErrorMessage = "Escolha o professor.")]
    public int? ProfessorId { get; set; }

    [Required(ErrorMessage = "Informe o nome da turma.")]
    [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o período (ex.: 2026/2).")]
    [StringLength(10, ErrorMessage = "O período deve ter no máximo 10 caracteres.")]
    public string Periodo { get; set; } = string.Empty;
}
