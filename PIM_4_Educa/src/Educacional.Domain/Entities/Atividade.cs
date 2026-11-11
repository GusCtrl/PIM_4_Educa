namespace Educacional.Domain.Entities;

public class Atividade
{
    public int Id { get; set; }
    public int TurmaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime Prazo { get; set; }

    public Turma Turma { get; set; } = null!;
}
