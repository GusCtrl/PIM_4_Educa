namespace Educacional.Application.Exceptions;

/// <summary>Violação de regra de negócio (a API responde 400).</summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem) { }
}

/// <summary>Usuário autenticado sem permissão para o recurso (403).</summary>
public class AcessoNegadoException : Exception
{
    public AcessoNegadoException(string mensagem) : base(mensagem) { }
}

/// <summary>Recurso não encontrado (404).</summary>
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem) { }
}

/// <summary>E-mail ou senha incorretos (401).</summary>
public class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException() : base("E-mail ou senha inválidos.") { }
}
