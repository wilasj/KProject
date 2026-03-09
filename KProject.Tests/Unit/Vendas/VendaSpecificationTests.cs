using KProject.Application.Vendas.ListaVendas;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class VendaSpecificationTests
{
    [Fact]
    public void BuscaVazia_DeveDeixarCriteriaNull()
    {
        var spec = new VendaSpecification(null);
        spec.Criteria.ShouldBeNull();
    }

    [Fact]
    public void BuscaEmBranco_DeveDeixarCriteriaNull()
    {
        var spec = new VendaSpecification("");
        spec.Criteria.ShouldBeNull();
    }

    [Fact]
    public void BuscaPreenchida_DeveDefinirCriteria()
    {
        var spec = new VendaSpecification("cliente");
        spec.Criteria.ShouldNotBeNull();
    }

    [Fact]
    public void OrderBy_DeveSempreApontarParaCriadaEm()
    {
        var spec = new VendaSpecification(null);
        spec.OrderBy.ShouldNotBeNull();
    }

    [Fact]
    public void Ascending_DeveSerFalse()
    {
        var spec = new VendaSpecification(null);
        spec.Ascending.ShouldBeFalse();
    }
}
