namespace Educacional.Domain.Entities;

public class Material
{
    public int Id { get; set; }
    public int DisciplinaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public Disciplina Disciplina { get; set; } = null!;
}
