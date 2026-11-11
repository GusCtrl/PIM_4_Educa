using Educacional.Application.Exceptions;

namespace Educacional.Api.Middleware;

/// <summary>Traduz as exceções da aplicação em respostas HTTP com uma mensagem clara.</summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, mensagem) = ex switch
            {
                CredenciaisInvalidasException e => (StatusCodes.Status401Unauthorized, e.Message),
                AcessoNegadoException e => (StatusCodes.Status403Forbidden, e.Message),
                NaoEncontradoException e => (StatusCodes.Status404NotFound, e.Message),
                RegraDeNegocioException e => (StatusCodes.Status400BadRequest, e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Erro não tratado");

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { erro = mensagem });
        }
    }
}
