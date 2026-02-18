using FluentValidation;

namespace KProject.Application.User.Register;

internal sealed class RegisterCommandValidator: AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
                .WithErrorCode("Register.EmailVazio")
                .WithMessage("O email não pode estar vazio")
            .EmailAddress()
                .WithErrorCode("Register.EmailInvalido")
                .WithMessage("O email é inválido");

        RuleFor(c => c.Password)
            .NotEmpty()
                .WithErrorCode("Register.SenhaVazia")
                .WithMessage("A senha não pode estar vazia");
    }
}