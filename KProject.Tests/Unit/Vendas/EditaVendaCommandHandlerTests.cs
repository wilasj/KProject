using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Vendas;
using KProject.Application.Vendas.EditaVenda;
using KProject.Common;
using KProject.Domain.Vendas;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class EditaVendaCommandHandlerTests
{
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Task<Result> Handle(EditaVendaCommand command) =>
        new EditaVendaCommandHandler(_vendas, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    private static EditaVendaCommand ComandoValido(int vendaId = 1, int itemId = 1) => new()
    {
        VendaId = vendaId,
        AlteradoPor = 1,
        Itens = [new EditaItemDto(itemId, 2u, 1u)]
    };

    private static Venda CriaVendaComItem()
    {
        var venda = Venda.Criar(1, 1, new Dictionary<(int, string), uint>
        {
            { (1, "Paciente Teste"), 10u }
        }).Value;
        return venda;
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoVendaNaoExiste()
    {
        _vendas.GetByIdWithItensAsync(99, Arg.Any<CancellationToken>()).Returns((Venda?)null);

        var result = await Handle(ComandoValido(vendaId: 99));

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.NaoEncontrada");
    }

    [Fact]
    public async Task Handle_DeveRetornarFailure_QuandoVendaFechada()
    {
        var venda = CriaVendaComItem();
        venda.FecharVenda();
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var result = await Handle(ComandoValido());

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.StatusInvalido");
    }

    [Fact]
    public async Task Handle_DeveRetornarFailure_QuandoItemNaoExisteNaVenda()
    {
        var venda = CriaVendaComItem();
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var command = new EditaVendaCommand
        {
            VendaId = 1,
            AlteradoPor = 1,
            Itens = [new EditaItemDto(999, 1u, 0u)]
        };

        var result = await Handle(command);

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.ItemNaoEncontrado");
    }

    [Fact]
    public async Task Handle_DeveRetornarFailure_QuandoVendidoMaisDevolvidoExcedeConsignada()
    {
        var venda = CriaVendaComItem();
        var itemId = venda.Itens.First().Id;
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var command = new EditaVendaCommand
        {
            VendaId = 1,
            AlteradoPor = 1,
            Itens = [new EditaItemDto(itemId, 8u, 5u)]
        };

        var result = await Handle(command);

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("ItemConsignado.HistoricoInvalido");
    }

    [Fact]
    public async Task Handle_DeveSalvarAlteracoes_QuandoEdicaoValida()
    {
        var venda = CriaVendaComItem();
        var itemId = venda.Itens.First().Id;
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var command = new EditaVendaCommand
        {
            VendaId = 1,
            AlteradoPor = 1,
            Itens = [new EditaItemDto(itemId, 3u, 2u)]
        };

        var result = await Handle(command);

        result.IsSuccess.ShouldBeTrue();
        venda.Itens.First().Vendido.ShouldBe(3u);
        venda.Itens.First().Devolvido.ShouldBe(2u);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
