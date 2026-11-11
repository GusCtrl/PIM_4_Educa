using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Educacional.Web.Filters;

/// <summary>
/// Captura ApiException que os controllers não trataram:
/// 401 (token expirado) volta para o login; os demais viram a tela de erro.
/// </summary>
public class ApiExceptionFilter : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not ApiException erro)
            return;

        if (erro.StatusCode == StatusCodes.Status401Unauthorized)
        {
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Result = new RedirectToActionResult("Login", "Account", new { sessaoExpirada = true });
        }
        else
        {
            context.Result = new ViewResult
            {
                ViewName = "Erro",
                StatusCode = erro.StatusCode,
                ViewData = new ViewDataDictionary<ErroViewModel>(new EmptyModelMetadataProvider(), context.ModelState)
                {
                    Model = new ErroViewModel(erro.StatusCode, erro.Message)
                }
            };
        }

        context.ExceptionHandled = true;
    }
}
