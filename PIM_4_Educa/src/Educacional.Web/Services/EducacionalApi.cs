using Educacional.Application.Dtos;

namespace Educacional.Web.Services;

/// <summary>Um método por endpoint da API. Os controllers do site só conhecem esta classe.</summary>
public class EducacionalApi
{
    private readonly ApiClient _api;

    public EducacionalApi(ApiClient api) => _api = api;

    // ---------- Autenticação ----------
    public Task<LoginResponse> LoginAsync(LoginRequest request) =>
        _api.PostAsync<LoginResponse>("api/auth/login", request);

    // ---------- Turmas ----------
    public Task<List<TurmaDto>> MinhasTurmasAsync() => _api.GetAsync<List<TurmaDto>>("api/turmas/minhas");
    public Task<List<TurmaDto>> TodasTurmasAsync() => _api.GetAsync<List<TurmaDto>>("api/turmas");
    public Task<TurmaDto> ObterTurmaAsync(int id) => _api.GetAsync<TurmaDto>($"api/turmas/{id}");
    public Task<List<AlunoTurmaDto>> AlunosAsync(int turmaId) => _api.GetAsync<List<AlunoTurmaDto>>($"api/turmas/{turmaId}/alunos");
    public Task<List<MediaAlunoDto>> MediaTurmaAsync(int turmaId) => _api.GetAsync<List<MediaAlunoDto>>($"api/turmas/{turmaId}/media");

    // ---------- Atividades e entregas ----------
    public Task<List<AtividadeDto>> AtividadesAsync(int turmaId) => _api.GetAsync<List<AtividadeDto>>($"api/turmas/{turmaId}/atividades");
    public Task<List<AtividadePendenteDto>> PendentesAsync() => _api.GetAsync<List<AtividadePendenteDto>>("api/atividades/pendentes");
    public Task<AtividadeDetalheDto> ObterAtividadeAsync(int id) => _api.GetAsync<AtividadeDetalheDto>($"api/atividades/{id}");
    public Task<List<EntregaAlunoDto>> EntregasDaAtividadeAsync(int id) => _api.GetAsync<List<EntregaAlunoDto>>($"api/atividades/{id}/entregas");
    public Task CriarAtividadeAsync(CriarAtividadeRequest request) => _api.PostAsync("api/atividades", request);
    public Task EnviarEntregaAsync(int atividadeId, string conteudo) => _api.PostAsync("api/entregas", new EnviarEntregaRequest(atividadeId, conteudo));
    public Task LancarNotaAsync(int entregaId, decimal nota) => _api.PutAsync($"api/entregas/{entregaId}/nota", new LancarNotaRequest(nota));
    public Task<List<BoletimItemDto>> BoletimAsync() => _api.GetAsync<List<BoletimItemDto>>("api/notas/boletim");

    // ---------- Avisos e materiais ----------
    public Task<List<AvisoDto>> AvisosAsync(int turmaId) => _api.GetAsync<List<AvisoDto>>($"api/turmas/{turmaId}/avisos");
    public Task CriarAvisoAsync(int turmaId, string texto) => _api.PostAsync($"api/turmas/{turmaId}/avisos", new CriarAvisoRequest(texto));
    public Task<List<MaterialDto>> MateriaisAsync(int turmaId) => _api.GetAsync<List<MaterialDto>>($"api/turmas/{turmaId}/materiais");
    public Task CriarMaterialAsync(int turmaId, string titulo, string url) => _api.PostAsync($"api/turmas/{turmaId}/materiais", new CriarMaterialRequest(titulo, url));

    // ---------- Administração ----------
    public Task<List<UsuarioDto>> UsuariosAsync(string? perfil = null) =>
        _api.GetAsync<List<UsuarioDto>>(string.IsNullOrEmpty(perfil) ? "api/usuarios" : $"api/usuarios?perfil={Uri.EscapeDataString(perfil)}");
    public Task CriarUsuarioAsync(CriarUsuarioRequest request) => _api.PostAsync("api/usuarios", request);
    public Task AlterarAtivoAsync(int id, bool ativo) => _api.PutAsync($"api/usuarios/{id}/ativo", new AlterarAtivoRequest(ativo));
    public Task<List<DisciplinaDto>> DisciplinasAsync() => _api.GetAsync<List<DisciplinaDto>>("api/disciplinas");
    public Task CriarDisciplinaAsync(CriarDisciplinaRequest request) => _api.PostAsync("api/disciplinas", request);
    public Task CriarTurmaAsync(CriarTurmaRequest request) => _api.PostAsync("api/turmas", request);
    public Task MatricularAsync(int turmaId, int alunoId) => _api.PostAsync("api/matriculas", new MatriculaRequest(turmaId, alunoId));
    public Task RemoverMatriculaAsync(int turmaId, int alunoId) => _api.DeleteAsync($"api/matriculas?turmaId={turmaId}&alunoId={alunoId}");
}
