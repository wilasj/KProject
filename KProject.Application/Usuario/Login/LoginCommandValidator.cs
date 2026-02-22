using FluentValidation;

namespace KProject.Application.Usuario.Login;

public sealed class LoginCommandValidator: AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithErrorCode("Login.EmailVazio")
            .WithMessage("O email não pode estar vazio")
            .EmailAddress()
            .WithErrorCode("Login.EmailInvalido")
            .WithMessage("O email é inválido");

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithErrorCode("Login.SenhaVazia")
            .WithMessage("A senha não pode estar vazia");
    }
}