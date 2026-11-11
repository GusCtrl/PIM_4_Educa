using System.Net.Http.Json;
using System.Text.Json;

namespace Educacional.Web.Services;

/// <summary>Erro devolvido pela API (ou falha ao falar com ela), já com mensagem pronta para o usuário.</summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(int statusCode, string mensagem) : base(mensagem) => StatusCode = statusCode;

    private record ErroApi(string? Erro);

    public static async Task<ApiException> DeRespostaAsync(HttpResponseMessage resposta, JsonSerializerOptions json)
    {
        var status = (int)resposta.StatusCode;
        string? mensagem = null;

        try
        {
            var corpo = await resposta.Content.ReadFromJsonAsync<ErroApi>(json);
            mensagem = corpo?.Erro;
        }
        catch
        {
            // corpo vazio ou que não é JSON (ex.: 401/403 sem corpo): usa a mensagem padrão abaixo
        }

        mensagem ??= status switch
        {
            400 => "Os dados enviados são inválidos.",
            401 => "Sua sessão expirou. Entre novamente.",
            403 => "Você não tem permissão para fazer isso.",
            404 => "Não encontrado.",
            _ => "Não foi possível concluir a operação. Tente novamente."
        };

        return new ApiException(status, mensagem);
    }
}
