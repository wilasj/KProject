using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Lotes;
using KProject.Application.Interfaces.Produtos;
using KProject.Application.Lotes;
using KProject.Application.Lotes.CriaLote;
using KProject.Application.Produtos;
using KProject.Application.Shared;
using KProject.Common;
using KProject.Domain.Lotes;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Lotes;

public class CriaLoteCommandHandlerTests
{
    private readonly IProdutoRepository _produtos = Substitute.For<IProdutoRepository>();
    private readonly ILoteRepository _lotes = Substitute.For<ILoteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Task<Result<int>> Handle(CriaLoteCommand command) =>
        new CriaLoteCommandHandler(_produtos, _lotes, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    [Fact]
    public async Task CriaLote_ProdutoNaoEncontrado_RetornaNotFound()
    {
        _produtos.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = 1,
            Numero = 1,
            Validade = new DateOnly(2027, 1, 1),
            CriadoPor = 1
        });

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task CriaLote_Valido_PersisteLoteERetornaId()
    {
        _produtos.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = 1,
            Numero = 1,
            Validade = new DateOnly(2027, 1, 1),
            QuantidadeInicial = 50,
            CriadoPor = 1
        });

        result.IsSuccess.ShouldBeTrue();
        await _lotes.Received(1).AddAsync(Arg.Any<Lote>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
