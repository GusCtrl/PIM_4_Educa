using System.Security.Claims;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public abstract class BaseController : Controller
{
    /// <summary>Perfil do usuário logado: Aluno, Professor ou Admin.</summary>
    protected string Perfil => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    protected void Sucesso(string mensagem) => TempData["Sucesso"] = mensagem;
    protected void Erro(string mensagem) => TempData["Erro"] = mensagem;

    /// <summary>Se o formulário for inválido, guarda as mensagens para a próxima tela e devolve true.</summary>
    protected bool ModeloInvalido()
    {
        if (ModelState.IsValid) return false;

        var mensagens = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(m => !string.IsNullOrWhiteSpace(m));

        Erro(string.Join(" ", mensagens));
        return true;
    }

    /// <summary>
    /// Executa uma chamada à API. Erros de regra (400/403/404) viram mensagem na tela;
    /// 401 e falhas de conexão continuam subindo e são tratados pelo ApiExceptionFilter.
    /// </summary>
    protected async Task<bool> TentarAsync(Func<Task> acao, string mensagemSucesso)
    {
        try
        {
            await acao();
            Sucesso(mensagemSucesso);
            return true;
        }
        catch (ApiException ex) when (ex.StatusCode is 400 or 403 or 404)
        {
            Erro(ex.Message);
            return false;
        }
    }
}
