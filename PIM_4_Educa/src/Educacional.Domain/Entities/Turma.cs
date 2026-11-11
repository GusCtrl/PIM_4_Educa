namespace Educacional.Domain.Entities;

public class Turma
{
    public int Id { get; set; }
    public int DisciplinaId { get; set; }
    public int ProfessorId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;

    public Disciplina Disciplina { get; set; } = null!;
    public Usuario Professor { get; set; } = null!;
}
