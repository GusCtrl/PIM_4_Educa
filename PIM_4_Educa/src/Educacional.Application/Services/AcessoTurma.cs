using Educacional.Application.Exceptions;
using Educacional.Application.Interfaces;
using Educacional.Domain;

namespace Educacional.Application.Services;

/// <summary>Regra única de "quem pode ver/gerir uma turma", reutilizada pelos serviços.</summary>
internal static class AcessoTurma
{
    /// <param name="somenteGestor">true = só o professor da turma ou o admin (ex.: ver notas de todos).</param>
    public static async Task GarantirAsync(ITurmaRepository turmas, int turmaId, int usuarioId,
                                           string perfil, bool somenteGestor = false)
    {
        bool permitido = perfil switch
        {
            Perfis.Admin => true,
            Perfis.Professor => await turmas.ProfessorLecionaAsync(turmaId, usuarioId),
            Perfis.Aluno => !somenteGestor && await turmas.AlunoMatriculadoAsync(turmaId, usuarioId),
            _ => false
        };

        if (!permitido)
            throw new AcessoNegadoException("Você não tem acesso a esta turma.");
    }
}
