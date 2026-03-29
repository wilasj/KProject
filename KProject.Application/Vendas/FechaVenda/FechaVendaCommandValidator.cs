using FluentValidation;

namespace KProject.Application.Vendas.FechaVenda;

public class FechaVendaCommandValidator : AbstractValidator<FechaVendaCommand>
{
    public FechaVendaCommandValidator()
    {
        RuleFor(x => x.VendaId)
            .GreaterThan(0)
            .WithErrorCode("Venda.IdInvalido")
            .WithMessage("O ID da venda deve ser maior que zero");

        RuleFor(x => x.FechadoPor)
            .GreaterThan(0)
            .WithErrorCode("Venda.UsuarioInvalido")
            .WithMessage("O ID do usuário deve ser maior que zero");
    }
}
