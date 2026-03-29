using KProject.Application.Interfaces.Lotes;
using KProject.Application.Lotes.HistoricoLote;
using KProject.Common;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Lotes;

public class HistoricoLoteQueryHandlerTests
{
    private readonly ILoteRepository _lotes = Substitute.For<ILoteRepository>();

    private Task<Result<HistoricoPage>> Handle(HistoricoLoteQuery query) =>
        new HistoricoLoteQueryHandler(_lotes)
            .Handle(query, TestContext.Current.CancellationToken);

    [Fact]
    public async Task HistoricoLote_LoteNaoEncontrado_RetornaNotFound()
    {
        _lotes.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await Handle(new HistoricoLoteQuery(99, 1, 20));

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.NotFound);
        result.Errors.First().Code.ShouldBe("Lote.NaoEncontrado");
    }

    [Fact]
    public async Task HistoricoLote_LoteEncontrado_RetornaPaginaDoRepositorio()
    {
        var page = new HistoricoPage(
        [
            new(1, "Entrada", 100, DateTime.UtcNow, null),
            new(2, "SaidaConsignacao", -1, DateTime.UtcNow.AddSeconds(-1), 42),
        ], HasMore: false);

        _lotes.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _lotes.GetHistoricoPagedAsync(1, 1, 20, Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new HistoricoLoteQuery(1, 1, 20));

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.HasMore.ShouldBeFalse();
    }

    [Fact]
    public async Task HistoricoLote_LoteEncontrado_RepassaParametrosDePaginacao()
    {
        var page = new HistoricoPage([], HasMore: true);

        _lotes.ExistsAsync(3, Arg.Any<CancellationToken>()).Returns(true);
        _lotes.GetHistoricoPagedAsync(3, 2, 10, Arg.Any<CancellationToken>()).Returns(page);

        var result = await Handle(new HistoricoLoteQuery(3, 2, 10));

        result.IsSuccess.ShouldBeTrue();
        await _lotes.Received(1).GetHistoricoPagedAsync(3, 2, 10, Arg.Any<CancellationToken>());
    }
}
