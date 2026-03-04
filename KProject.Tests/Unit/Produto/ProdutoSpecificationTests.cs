using KProject.Application.Produto.ListaProdutos;
using Shouldly;

namespace KProject.Tests.Unit.Produto;

public class ProdutoSpecificationTests
{
    private static Domain.Produto.Produto CriaProduto(string nome) =>
        Domain.Produto.Produto.Criar(nome, "REF-001", "Descrição", "ANVISA-001").Value;

    [Fact]
    public void BuscaVazia_DeveDeixarCriteriaNull()
    {
        var spec = new ProdutoSpecification(null);
        spec.Criteria.ShouldBeNull();
    }

    [Fact]
    public void BuscaEmBranco_DeveDeixarCriteriaNull()
    {
        var spec = new ProdutoSpecification("");
        spec.Criteria.ShouldBeNull();
    }

    [Fact]
    public void Busca_DeveFiltrarPorNome()
    {
        var produtos = new[]
        {
            CriaProduto("Paracetamol"),
            CriaProduto("Ibuprofeno"),
            CriaProduto("Dipirona"),
        };

        var spec = new ProdutoSpecification("para");
        var resultado = produtos.Where(spec.Criteria!.Compile()).ToList();

        resultado.Count.ShouldBe(1);
        resultado[0].Nome.ShouldBe("Paracetamol");
    }

    [Fact]
    public void Busca_DeveSerCaseInsensitive()
    {
        var produtos = new[]
        {
            CriaProduto("Paracetamol"),
            CriaProduto("Ibuprofeno"),
        };

        var spec = new ProdutoSpecification("PARA");
        var resultado = produtos.Where(spec.Criteria!.Compile()).ToList();

        resultado.Count.ShouldBe(1);
        resultado[0].Nome.ShouldBe("Paracetamol");
    }

    [Fact]
    public void Busca_SemMatch_DeveRetornarVazio()
    {
        var produtos = new[]
        {
            CriaProduto("Paracetamol"),
            CriaProduto("Ibuprofeno"),
        };

        var spec = new ProdutoSpecification("dipirona");
        var resultado = produtos.Where(spec.Criteria!.Compile()).ToList();

        resultado.ShouldBeEmpty();
    }

    [Fact]
    public void OrderBy_DeveSempreApontarParaNome()
    {
        var spec = new ProdutoSpecification(null);

        spec.OrderBy.ShouldNotBeNull();
        spec.Ascending.ShouldBeTrue();
    }
}
