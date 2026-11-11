using System.Globalization;
using Educacional.Domain;
using Educacional.Web.Models;
using Educacional.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educacional.Web.Controllers;

public class EntregasController : BaseController
{
    private readonly EducacionalApi _api;

    public EntregasController(EducacionalApi api) => _api = api;

    [Authorize(Roles = Perfis.Aluno)]
    [HttpPost]
    public async Task<IActionResult> Enviar(EnviarEntregaViewModel modelo)
    {
        if (!ModeloInvalido())
            await TentarAsync(() => _api.EnviarEntregaAsync(modelo.AtividadeId, modelo.Conteudo),
                              "Entrega enviada com sucesso.");

        return RedirectToAction("Detalhes", "Atividades", new { id = modelo.AtividadeId });
    }

    [Authorize(Roles = Perfis.Professor)]
    [HttpPost]
    public async Task<IActionResult> LancarNota(int entregaId, int atividadeId, string? nota)
    {
        // Aceita "8,5" e "8.5", independentemente da cultura do servidor.
        var texto = (nota ?? string.Empty).Trim().Replace(',', '.');

        if (!decimal.TryParse(texto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var valor))
            Erro("Informe uma nota válida, de 0 a 10.");
        else
            await TentarAsync(() => _api.LancarNotaAsync(entregaId, valor), "Nota salva.");

        return RedirectToAction("Detalhes", "Atividades", new { id = atividadeId });
    }
}
