namespace Educacional.Application.Dtos;

// ---------- Autenticação ----------
public record LoginRequest(string Email, string Senha);
public record LoginResponse(string Token, int UsuarioId, string Nome, string Perfil);

// ---------- Turmas e atividades ----------
public record TurmaDto(int Id, string Nome, string Disciplina, string Periodo, string Professor);
public record AlunoTurmaDto(int AlunoId, string Nome, string Email);
public record AtividadeDto(int Id, int TurmaId, string Titulo, string? Descricao, DateTime Prazo);
public record CriarAtividadeRequest(int TurmaId, string Titulo, string? Descricao, DateTime Prazo);
public record AtividadeDetalheDto(int Id, int TurmaId, string Turma, string Titulo, string? Descricao,
                                  DateTime Prazo, MinhaEntregaDto? MinhaEntrega);

// ---------- Entregas e notas ----------
public record EnviarEntregaRequest(int AtividadeId, string Conteudo);
public record EntregaDto(int Id, int AtividadeId, DateTime DataEnvio, decimal? Nota);
public record MinhaEntregaDto(int Id, string Conteudo, DateTime DataEnvio, decimal? Nota);
public record EntregaAlunoDto(int Id, int AlunoId, string Aluno, string Conteudo, DateTime DataEnvio, decimal? Nota);
public record LancarNotaRequest(decimal Nota);

// ---------- Avisos e materiais ----------
public record AvisoDto(int Id, int TurmaId, string Autor, string Texto, DateTime DataPublicacao);
public record CriarAvisoRequest(string Texto);
public record MaterialDto(int Id, int DisciplinaId, string Titulo, string Url);
public record CriarMaterialRequest(string Titulo, string Url);

// ---------- Administração ----------
public record UsuarioDto(int Id, string Nome, string Email, string Perfil, bool Ativo);
public record CriarUsuarioRequest(string Nome, string Email, string Senha, string Perfil);
public record AlterarAtivoRequest(bool Ativo);
public record DisciplinaDto(int Id, string Nome, string? Descricao);
public record CriarDisciplinaRequest(string Nome, string? Descricao);
public record CriarTurmaRequest(int DisciplinaId, int ProfessorId, string Nome, string Periodo);
public record MatriculaRequest(int TurmaId, int AlunoId);

// ---------- Resultados de stored procedures ----------
// (classes com propriedades de mesmo nome das colunas retornadas pela procedure)
public class AtividadePendenteDto
{
    public int AtividadeId { get; set; }
    public int TurmaId { get; set; }
    public string Turma { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public DateTime Prazo { get; set; }
}

public class BoletimItemDto
{
    public int TurmaId { get; set; }
    public string Turma { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public int TotalAtividades { get; set; }
    public int TotalEntregas { get; set; }
    public decimal? Media { get; set; }
}

public class MediaAlunoDto
{
    public int AlunoId { get; set; }
    public string Aluno { get; set; } = string.Empty;
    public int Entregas { get; set; }
    public decimal? Media { get; set; }
}
