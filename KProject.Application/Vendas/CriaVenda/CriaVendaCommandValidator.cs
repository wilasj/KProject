using FluentValidation;

namespace KProject.Application.Vendas.CriaVenda;

public sealed class CriaVendaCommandValidator : AbstractValidator<CriaVendaCommand>
{
    public CriaVendaCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.ClienteId)
            .GreaterThan(0)
                .WithErrorCode("CriaVenda.ClienteInvalido")
                .WithMessage("O ID do cliente deve ser maior que zero");

        RuleFor(c => c.CriadaPor)
            .GreaterThan(0)
                .WithErrorCode("Venda.UsuarioInvalido")
                .WithMessage("O ID do usuário deve ser maior que zero");

        RuleFor(c => c.Itens)
            .NotEmpty()
                .WithErrorCode("CriaVenda.ItensVazios")
                .WithMessage("A venda deve ter ao menos um item");

        RuleFor(c => c.Itens)
            .Must(itens => itens
                .GroupBy(i => (i.LoteId, i.PacienteNome.ToUpperInvariant()))
                .All(g => g.Count() == 1))
                .WithErrorCode("Venda.ItemDuplicado")
                .WithMessage("A lista contém itens duplicados (mesmo lote e paciente)");

        RuleForEach(c => c.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.LoteId)
                .GreaterThan(0)
                    .WithErrorCode("CriaVenda.LoteInvalido")
                    .WithMessage("O ID do lote deve ser maior que zero");

            item.RuleFor(i => i.PacienteNome)
                .NotEmpty()
                    .WithErrorCode("CriaVenda.PacienteNomeInvalido")
                    .WithMessage("O nome do paciente é obrigatório")
                .MaximumLength(200)
                    .WithErrorCode("CriaVenda.PacienteNomeMuitoLongo")
                    .WithMessage("O nome do paciente deve ter no máximo 200 caracteres");

            item.RuleFor(i => i.Quantidade)
                .GreaterThan(0u)
                    .WithErrorCode("CriaVenda.QuantidadeInvalida")
                    .WithMessage("A quantidade deve ser maior que zero");
        });
    }
}
