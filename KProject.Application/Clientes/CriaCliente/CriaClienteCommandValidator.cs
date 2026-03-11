using FluentValidation;

namespace KProject.Application.Clientes.CriaCliente;

public sealed class CriaClienteCommandValidator : AbstractValidator<CriaClienteCommand>
{
    public CriaClienteCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Nome)
            .NotEmpty()
                .WithErrorCode("CriaCliente.NomeVazio")
                .WithMessage("O nome não pode estar vazio")
            .MaximumLength(200)
                .WithErrorCode("CriaCliente.NomeMuitoLongo")
                .WithMessage("O nome não pode ter mais de 200 caracteres");
    }
}
