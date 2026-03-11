using KProject.Application.Clientes.ListaClientes;
using KProject.Application.Interfaces.Clientes;
using KProject.Application.Shared;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Clientes;

public class ListaClientesQueryHandlerTests
{
    private readonly IClienteRepository _clientes = Substitute.For<IClienteRepository>();

    private Task<KProject.Common.Result<Page<ClienteResponse>>> Handle(ListaClientesQuery query) =>
        new ListaClientesQueryHandler(_clientes)
            .Handle(query, TestContext.Current.CancellationToken);

    [Fact]
    public async Task ListaClientes_RepassaParametrosParaRepositorio()
    {
        var page = new Page<ClienteResponse>([], 0);
        _clientes.GetPagedAsync("busca", 2, 5, Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new ListaClientesQuery { Busca = "busca", Page = 2, PageSize = 5 });

        result.IsSuccess.ShouldBeTrue();
        await _clientes.Received(1).GetPagedAsync("busca", 2, 5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListaClientes_RetornaPaginaDoRepositorio()
    {
        var itens = new List<ClienteResponse> { new(1, "João Silva") };
        var page = new Page<ClienteResponse>(itens, 1);
        _clientes.GetPagedAsync(Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new ListaClientesQuery());

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Total.ShouldBe(1);
    }
}
