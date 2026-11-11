namespace Educacional.Domain;

/// <summary>Perfis de acesso do sistema (iguais aos valores aceitos no banco).</summary>
public static class Perfis
{
    public const string Aluno = "Aluno";
    public const string Professor = "Professor";
    public const string Admin = "Admin";

    public static readonly string[] Todos = { Aluno, Professor, Admin };
}
