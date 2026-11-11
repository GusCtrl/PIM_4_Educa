namespace Educacional.Domain.Entities;

public class Matricula
{
    public int Id { get; set; }
    public int TurmaId { get; set; }
    public int AlunoId { get; set; }
    public DateTime DataMatricula { get; set; }

    public Turma Turma { get; set; } = null!;
    public Usuario Aluno { get; set; } = null!;
}
