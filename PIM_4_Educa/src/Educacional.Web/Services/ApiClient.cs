using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Educacional.Web.Services;

/// <summary>
/// Cliente HTTP da API. Anexa automaticamente o token JWT do usuário logado
/// (guardado no cookie de login) em cada chamada.
/// </summary>
public class ApiClient
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _contexto;

    public ApiClient(HttpClient http, IHttpContextAccessor contexto)
    {
        _http = http;
        _contexto = contexto;
    }

    public async Task<T> GetAsync<T>(string url)
    {
        using var resposta = await EnviarAsync(HttpMethod.Get, url, null);
        return (await resposta.Content.ReadFromJsonAsync<T>(Json))!;
    }

    public async Task<T> PostAsync<T>(string url, object corpo)
    {
        using var resposta = await EnviarAsync(HttpMethod.Post, url, corpo);
        return (await resposta.Content.ReadFromJsonAsync<T>(Json))!;
    }

    public async Task PostAsync(string url, object corpo)
    {
        using var resposta = await EnviarAsync(HttpMethod.Post, url, corpo);
    }

    public async Task PutAsync(string url, object corpo)
    {
        using var resposta = await EnviarAsync(HttpMethod.Put, url, corpo);
    }

    public async Task DeleteAsync(string url)
    {
        using var resposta = await EnviarAsync(HttpMethod.Delete, url, null);
    }

    private async Task<HttpResponseMessage> EnviarAsync(HttpMethod metodo, string url, object? corpo)
    {
        using var requisicao = new HttpRequestMessage(metodo, url);

        var token = _contexto.HttpContext is null
            ? null
            : await _contexto.HttpContext.GetTokenAsync("access_token");
        if (!string.IsNullOrEmpty(token))
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (corpo is not null)
            requisicao.Content = JsonContent.Create(corpo, options: Json);

        HttpResponseMessage resposta;
        try
        {
            resposta = await _http.SendAsync(requisicao);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new ApiException(503, "Não foi possível conectar à API. Verifique se ela está em execução.");
        }

        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await ApiException.DeRespostaAsync(resposta, Json);
            resposta.Dispose();
            throw erro;
        }

        return resposta;
    }
}
