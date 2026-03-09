using KProject.Application.Interfaces.Vendas;
using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;
using KProject.Common;
using KProject.Domain.Vendas;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class ListaVendasQueryHandlerTests
{
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();

    private Task<Result<Page<VendaResponse>>> Handle(ListaVendasQuery query) =>
        new ListaVendasQueryHandler(_vendas)
            .Handle(query, TestContext.Current.CancellationToken);

    [Fact]
    public async Task ListaVendas_Repassa_ParametrosParaRepositorio()
    {
        var page = new Page<VendaResponse>([], 0);
        _vendas.GetPagedAsync("cliente", 2, 5, Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new ListaVendasQuery { Busca = "cliente", Page = 2, PageSize = 5 });

        result.IsSuccess.ShouldBeTrue();
        await _vendas.Received(1).GetPagedAsync("cliente", 2, 5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListaVendas_RetornaPaginaDoRepositorio()
    {
        var itens = new List<VendaResponse>
        {
            new(1, "Cliente Teste", DateTime.UtcNow, StatusVenda.Aberta, 3),
        };
        var page = new Page<VendaResponse>(itens, 1);
        _vendas.GetPagedAsync(Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new ListaVendasQuery());

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Total.ShouldBe(1);
    }
}
