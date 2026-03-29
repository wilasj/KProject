using FluentValidation;

namespace KProject.Application.Vendas.CancelaVenda;

public class CancelaVendaCommandValidator : AbstractValidator<CancelaVendaCommand>
{
    public CancelaVendaCommandValidator()
    {
        RuleFor(x => x.VendaId)
            .GreaterThan(0)
            .WithErrorCode("Venda.IdInvalido")
            .WithMessage("O ID da venda deve ser maior que zero");

        RuleFor(x => x.CanceladoPor)
            .GreaterThan(0)
            .WithErrorCode("Venda.UsuarioInvalido")
            .WithMessage("O ID do usuário deve ser maior que zero");
    }
}
