using FluentValidation;

namespace KProject.Application.Vendas.EditaVenda;

public sealed class EditaVendaCommandValidator : AbstractValidator<EditaVendaCommand>
{
    public EditaVendaCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.VendaId)
            .GreaterThan(0)
                .WithErrorCode("EditaVenda.VendaInvalida")
                .WithMessage("O ID da venda deve ser maior que zero");

        RuleFor(c => c.Itens)
            .NotEmpty()
                .WithErrorCode("EditaVenda.ItensVazios")
                .WithMessage("A lista de itens não pode estar vazia");

        RuleFor(c => c.Itens)
            .Must(itens => itens
                .GroupBy(i => i.Id)
                .All(g => g.Count() == 1))
                .WithErrorCode("EditaVenda.ItemDuplicado")
                .WithMessage("A lista contém itens duplicados");

        RuleForEach(c => c.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.Id)
                .GreaterThan(0)
                    .WithErrorCode("EditaVenda.ItemIdInvalido")
                    .WithMessage("O ID do item deve ser maior que zero");
        });
    }
}
