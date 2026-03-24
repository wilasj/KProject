using KProject.Application.Interfaces.Vendas;
using KProject.Application.Vendas.ObtemVenda;
using KProject.Common;
using KProject.Domain.Vendas;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class ObtemVendaQueryHandlerTests
{
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();

    private Task<Result<VendaDetalheResponse>> Handle(ObtemVendaQuery query) =>
        new ObtemVendaQueryHandler(_vendas)
            .Handle(query, TestContext.Current.CancellationToken);

    [Fact]
    public async Task ObtemVenda_RetornaNotFound_QuandoVendaNaoExiste()
    {
        _vendas.GetDetalheAsync(99, Arg.Any<CancellationToken>()).Returns((VendaDetalheResponse?)null);

        var result = await Handle(new ObtemVendaQuery(99));

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task ObtemVenda_RetornaVenda_QuandoEncontrada()
    {
        var response = new VendaDetalheResponse(
            1, StatusVenda.Aberta, DateTime.UtcNow, "user@test.com",
            null, "Cliente Teste", 10u, 3u, 2u, []);

        _vendas.GetDetalheAsync(1, Arg.Any<CancellationToken>()).Returns(response);

        var result = await Handle(new ObtemVendaQuery(1));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(response);
    }
}
