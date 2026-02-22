using Microsoft.AspNetCore.Identity;

namespace KProject.Infrastructure.Shared;

public class ErrorDescriber: IdentityErrorDescriber
{
    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError()
        {
            Code = "Usuario.FalhaDeConcorrencia",
            Description = "Uma falha de concorrência no sistema ocorreu."
        };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError()
        {
            Code = "Usuario.SenhasDiferem",
            Description = "As senhas diferem."
        };
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError()
        {
            Code = "Usuario.TokenInvalido",
            Description = "O token recebido é inválido."
        };
    }

    public override IdentityError InvalidUserName(string? userName)
    {
        return new IdentityError()
        {
            Code = "Usuario.UsernameInvalido",
            Description = "O nome de usuário inserido é inválido."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError()
        {
            Code = "Usuario.EmailDuplicado",
            Description = "O email inserido já está sendo utilizado."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError()
        {
            Code = "Usuario.EmailDuplicado",
            Description = "O email inserido já está sendo utilizado."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError()
        {
            Code = "Usuario.SenhaCurta",
            Description = "A senha não tem o tamanho mínimo necessário."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError()
        {
            Code = "Usuario.SemNaoAlfanumericos",
            Description = "A senha precisa conter caracteres não-alfanuméricos (@, !, _)."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError()
        {
            Code = "Usuario.SemNumeros",
            Description = "A senha precisa conter números."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError()
        {
            Code = "Usuario.SemLetrasMinusculas",
            Description = "A senha precisa conter letras minúsculas."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError()
        {
            Code = "Usuario.SemLetrasMaiusculas",
            Description = "A senha precisa conter letras maiúsculas."
        };
    }
}
