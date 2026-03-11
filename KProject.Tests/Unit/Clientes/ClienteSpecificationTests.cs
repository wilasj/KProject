using KProject.Application.Clientes.ListaClientes;
using KProject.Domain.Clientes;
using Shouldly;

namespace KProject.Tests.Unit.Clientes;

public class ClienteSpecificationTests
{
    private static Cliente CriaCliente(string nome) => Cliente.Criar(nome);

    [Fact]
    public void BuscaVazia_DeveDeixarCriteriaNull()
    {
        var spec = new ClienteSpecification(null);
        spec.Criteria.ShouldBeNull();
    }

    [Fact]
    public void BuscaEmBranco_DeveDeixarCriteriaNull()
    {
        var spec = new ClienteSpecification("");
        spec.Criteria.ShouldBeNull();
    }

    [Fact]
    public void Busca_DeveFiltrarPorNome()
    {
        var clientes = new[]
        {
            CriaCliente("Farmácia Central"),
            CriaCliente("Drogaria Sul"),
            CriaCliente("Farmacinha do Bairro"),
        };

        var spec = new ClienteSpecification("farmá");
        var resultado = clientes.Where(spec.Criteria!.Compile()).ToList();

        resultado.Count.ShouldBe(1);
        resultado.ShouldAllBe(c => c.Nome.ToLower().Contains("farmá"));
    }

    [Fact]
    public void Busca_DeveSerCaseInsensitive()
    {
        var clientes = new[]
        {
            CriaCliente("Farmácia Central"),
            CriaCliente("Drogaria Sul"),
        };

        var spec = new ClienteSpecification("FARMÁCIA");
        var resultado = clientes.Where(spec.Criteria!.Compile()).ToList();

        resultado.Count.ShouldBe(1);
        resultado[0].Nome.ShouldBe("Farmácia Central");
    }

    [Fact]
    public void Busca_SemMatch_DeveRetornarVazio()
    {
        var clientes = new[]
        {
            CriaCliente("Farmácia Central"),
            CriaCliente("Drogaria Sul"),
        };

        var spec = new ClienteSpecification("supermercado");
        var resultado = clientes.Where(spec.Criteria!.Compile()).ToList();

        resultado.ShouldBeEmpty();
    }

    [Fact]
    public void OrderBy_DeveSempreApontarParaNome()
    {
        var spec = new ClienteSpecification(null);

        spec.OrderBy.ShouldNotBeNull();
        spec.Ascending.ShouldBeTrue();
    }
}
