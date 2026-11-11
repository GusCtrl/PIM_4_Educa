namespace Educacional.Domain.Entities;

public class Entrega
{
    public int Id { get; set; }
    public int AtividadeId { get; set; }
    public int AlunoId { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public decimal? Nota { get; set; }

    public Atividade Atividade { get; set; } = null!;
    public Usuario Aluno { get; set; } = null!;
}
