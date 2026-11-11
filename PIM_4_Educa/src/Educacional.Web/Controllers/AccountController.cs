using System.Security.Claims;
using Educacional.Application.Dtos;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public class AccountController : BaseController
{
    private readonly EducacionalApi _api;

    public AccountController(EducacionalApi api) => _api = api;

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        LoginResponse resposta;
        try
        {
            resposta = await _api.LoginAsync(new LoginRequest(model.Email.Trim(), model.Senha));
        }
        catch (ApiException ex) when (ex.StatusCode is 400 or 401)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, resposta.UsuarioId.ToString()),
            new(ClaimTypes.Name, resposta.Nome),
            new(ClaimTypes.Role, resposta.Perfil)
        };
        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // O token JWT da API fica guardado no cookie e é reenviado pelo ApiClient a cada chamada.
        var propriedades = new AuthenticationProperties();
        propriedades.StoreTokens(new[] { new AuthenticationToken { Name = "access_token", Value = resposta.Token } });

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidade),
            propriedades);

        return LocalRedirect(Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl! : "/");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();
}
