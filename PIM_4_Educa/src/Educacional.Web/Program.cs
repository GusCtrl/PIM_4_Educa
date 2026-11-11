using Educacional.Web.Filters;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var apiUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Api:BaseUrl não configurada.");

// ---------- Acesso à API ----------
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl.TrimEnd('/') + "/");   // a barra final é necessária para URLs relativas
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<EducacionalApi>();

// ---------- Login por cookie (o token JWT da API fica guardado dentro do cookie) ----------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "educacional.auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120);   // mesmo prazo do JWT (Jwt:ExpireMinutes da API)
        options.SlidingExpiration = false;
    });
builder.Services.AddAuthorization();

// ---------- MVC ----------
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());                 // tudo exige login, exceto [AllowAnonymous]
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // proteção CSRF em todo POST
    options.Filters.Add<ApiExceptionFilter>();                  // traduz erros da API em telas amigáveis
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

// Números e datas em português do Brasil
var cultura = new Microsoft.AspNetCore.Builder.RequestLocalizationOptions()
    .SetDefaultCulture("pt-BR")
    .AddSupportedCultures("pt-BR")
    .AddSupportedUICultures("pt-BR");
app.UseRequestLocalization(cultura);

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
