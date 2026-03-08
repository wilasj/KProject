using FluentValidation;

namespace KProject.Application.Lotes.CriaLote;

public sealed class CriaLoteCommandValidator : AbstractValidator<CriaLoteCommand>
{
    public CriaLoteCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.ProdutoId)
            .GreaterThan(0)
                .WithErrorCode("CriaLote.ProdutoInvalido")
                .WithMessage("O ID do produto deve ser maior que zero");

        RuleFor(c => c.Numero)
            .GreaterThan(0)
                .WithErrorCode("CriaLote.NumeroInvalido")
                .WithMessage("O número do lote deve ser maior que zero");

        RuleFor(c => c.Validade)
            .NotEqual(DateOnly.MinValue)
                .WithErrorCode("CriaLote.ValidadeInvalida")
                .WithMessage("A validade do lote deve ser informada");
    }
}
