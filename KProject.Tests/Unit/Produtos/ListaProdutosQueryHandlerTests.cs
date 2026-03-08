using KProject.Application.Interfaces.Produtos;
using KProject.Application.Produtos.ListaProdutos;
using KProject.Application.Shared;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Produtos;

public class ListaProdutosQueryHandlerTests
{
    private readonly IProdutoRepository _produtos = Substitute.For<IProdutoRepository>();

    private Task<KProject.Common.Result<Page<ProdutoResponse>>> Handle(ListaProdutosQuery query) =>
        new ListaProdutosQueryHandler(_produtos)
            .Handle(query, TestContext.Current.CancellationToken);

    [Fact]
    public async Task ListaProdutos_Repassa_ParametrosParaRepositorio()
    {
        var page = new Page<ProdutoResponse>([], 0);
        _produtos.GetPagedAsync("busca", 2, 5, Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new ListaProdutosQuery { Busca = "busca", Page = 2, PageSize = 5 });

        result.IsSuccess.ShouldBeTrue();
        await _produtos.Received(1).GetPagedAsync("busca", 2, 5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListaProdutos_RetornaPaginaDoRepositorio()
    {
        var itens = new List<ProdutoResponse>
        {
            new(1, "Paracetamol", "REF-01", "Desc", "ANVISA-001", DateTime.UtcNow),
        };
        var page = new Page<ProdutoResponse>(itens, 1);
        _produtos.GetPagedAsync(Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new ListaProdutosQuery());

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Total.ShouldBe(1);
    }
}
