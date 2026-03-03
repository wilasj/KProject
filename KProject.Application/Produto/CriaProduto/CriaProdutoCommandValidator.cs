using FluentValidation;

namespace KProject.Application.Produto.CriaProduto;

public sealed class CriaProdutoCommandValidator : AbstractValidator<CriaProdutoCommand>
{
    public CriaProdutoCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Nome)
            .NotEmpty()
                .WithErrorCode("CriaProduto.NomeVazio")
                .WithMessage("O nome não pode estar vazio")
            .MaximumLength(100)
                .WithErrorCode("CriaProduto.NomeMuitoLongo")
                .WithMessage("O nome não pode ter mais de 100 caracteres");

        RuleFor(c => c.Referencia)
            .NotEmpty()
                .WithErrorCode("CriaProduto.ReferenciaVazia")
                .WithMessage("A referência não pode estar vazia")
            .MaximumLength(100)
                .WithErrorCode("CriaProduto.ReferenciaMuitoLonga")
                .WithMessage("A referência não pode ter mais de 100 caracteres");

        RuleFor(c => c.Descricao)
            .NotEmpty()
                .WithErrorCode("CriaProduto.DescricaoVazia")
                .WithMessage("A descrição não pode estar vazia")
            .MaximumLength(300)
                .WithErrorCode("CriaProduto.DescricaoMuitoLonga")
                .WithMessage("A descrição não pode ter mais de 300 caracteres");

        RuleFor(c => c.CodigoAnvisa)
            .NotEmpty()
                .WithErrorCode("CriaProduto.CodigoAnvisaVazio")
                .WithMessage("O código ANVISA não pode estar vazio")
            .MaximumLength(100)
                .WithErrorCode("CriaProduto.CodigoAnvisaMuitoLongo")
                .WithMessage("O código ANVISA não pode ter mais de 100 caracteres");
    }
}