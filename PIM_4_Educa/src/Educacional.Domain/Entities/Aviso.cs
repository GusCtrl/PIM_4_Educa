namespace Educacional.Domain.Entities;

public class Aviso
{
    public int Id { get; set; }
    public int TurmaId { get; set; }
    public int AutorId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime DataPublicacao { get; set; }

    public Turma Turma { get; set; } = null!;
    public Usuario Autor { get; set; } = null!;
}
