using KProject.Application.Interfaces.Lotes;
using KProject.Application.Interfaces.Produtos;
using KProject.Application.Lotes.ListaLotes;
using KProject.Common;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Lotes;

public class ListaLotesQueryHandlerTests
{
    private readonly IProdutoRepository _produtos = Substitute.For<IProdutoRepository>();
    private readonly ILoteRepository _lotes = Substitute.For<ILoteRepository>();

    private Task<Result<IReadOnlyList<LoteResponse>>> Handle(ListaLotesQuery query) =>
        new ListaLotesQueryHandler(_lotes, _produtos)
            .Handle(query, TestContext.Current.CancellationToken);

    [Fact]
    public async Task ListaLotes_ProdutoNaoEncontrado_RetornaNotFound()
    {
        _produtos.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await Handle(new ListaLotesQuery(99));

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task ListaLotes_ProdutoEncontrado_RetornaLotes()
    {
        var lotes = new List<LoteResponse>
        {
            new(1, 101, new DateOnly(2027, 3, 15), 10),
            new(2, 102, new DateOnly(2027, 9, 1), 20),
        };

        _produtos.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _lotes.GetByProdutoIdAsync(1, Arg.Any<CancellationToken>()).Returns((IReadOnlyList<LoteResponse>)lotes);

        var result = await Handle(new ListaLotesQuery(1));

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListaLotes_SemLotes_RetornaListaVazia()
    {
        _produtos.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _lotes.GetByProdutoIdAsync(1, Arg.Any<CancellationToken>()).Returns((IReadOnlyList<LoteResponse>)new List<LoteResponse>());

        var result = await Handle(new ListaLotesQuery(1));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }
}
