using Educacional.Application.Dtos;
using Educacional.Domain.Entities;

namespace Educacional.Application.Interfaces;

// ---------- Repositórios (implementados na camada Infrastructure) ----------
public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<List<UsuarioDto>> ListarAsync(string? perfil);
    Task<bool> EmailExisteAsync(string email);
    Task<UsuarioDto> AdicionarAsync(Usuario usuario);
    Task<bool> AlterarAtivoAsync(int id, bool ativo);
}

public interface ITurmaRepository
{
    Task<List<TurmaDto>> ListarTodasAsync();
    Task<List<TurmaDto>> ListarDoAlunoAsync(int alunoId);
    Task<List<TurmaDto>> ListarDoProfessorAsync(int professorId);
    Task<TurmaDto?> ObterDtoAsync(int turmaId);
    Task<Turma?> ObterPorIdAsync(int turmaId);
    Task<TurmaDto> AdicionarAsync(Turma turma);
    Task<List<AlunoTurmaDto>> ListarAlunosAsync(int turmaId);
    Task<bool> AlunoMatriculadoAsync(int turmaId, int alunoId);
    Task<bool> ProfessorLecionaAsync(int turmaId, int professorId);
}

public interface IMatriculaRepository
{
    Task<bool> ExisteAsync(int turmaId, int alunoId);
    Task AdicionarAsync(Matricula matricula);
    Task<bool> RemoverAsync(int turmaId, int alunoId);
}

public interface IDisciplinaRepository
{
    Task<List<DisciplinaDto>> ListarAsync();
    Task<bool> ExisteAsync(int id);
    Task<bool> NomeExisteAsync(string nome);
    Task<DisciplinaDto> AdicionarAsync(Disciplina disciplina);
}

public interface IAtividadeRepository
{
    Task<List<AtividadeDto>> ListarPorTurmaAsync(int turmaId);
    Task<Atividade?> ObterPorIdAsync(int id);                            // já traz a Turma
    Task<AtividadeDto> AdicionarAsync(Atividade atividade);
    Task<List<AtividadePendenteDto>> ListarPendentesAsync(int alunoId);  // sp_AtividadesPendentes
}

public interface IEntregaRepository
{
    Task<bool> ExisteAsync(int atividadeId, int alunoId);
    Task<EntregaDto> AdicionarAsync(Entrega entrega);
    Task<MinhaEntregaDto?> ObterMinhaAsync(int atividadeId, int alunoId);
    Task<List<EntregaAlunoDto>> ListarPorAtividadeAsync(int atividadeId);
    Task LancarNotaAsync(int entregaId, decimal nota, int professorId);  // sp_LancarNota
    Task<List<BoletimItemDto>> ObterBoletimAsync(int alunoId);            // sp_BoletimAluno
    Task<List<MediaAlunoDto>> ObterMediaTurmaAsync(int turmaId);          // sp_MediaTurma
}

public interface IAvisoRepository
{
    Task<List<AvisoDto>> ListarPorTurmaAsync(int turmaId);
    Task<AvisoDto> AdicionarAsync(Aviso aviso);
}

public interface IMaterialRepository
{
    Task<List<MaterialDto>> ListarPorDisciplinaAsync(int disciplinaId);
    Task<MaterialDto> AdicionarAsync(Material material);
}

// ---------- Serviços de infraestrutura usados pela aplicação ----------
public interface IPasswordHasher
{
    string Gerar(string senha);
    bool Verificar(string senha, string hash);
}

public interface ITokenService
{
    string GerarToken(Usuario usuario);
}
