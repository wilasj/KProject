using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Estoques;
using KProject.Application.Interfaces.Vendas;
using KProject.Application.Vendas.FechaVenda;
using KProject.Common;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Vendas;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class FechaVendaCommandHandlerTests
{
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();
    private readonly IEstoqueRepository _estoques = Substitute.For<IEstoqueRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Task<Result> Handle(FechaVendaCommand command) =>
        new FechaVendaCommandHandler(_vendas, _estoques, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    private static FechaVendaCommand ComandoValido(int vendaId = 1) => new()
    {
        VendaId = vendaId,
        FechadoPor = 1
    };

    private static Venda CriaVendaAberta()
    {
        return Venda.Criar(1, 1, new Dictionary<(int, string), uint>
        {
            { (1, "Paciente Teste"), 10u }
        }).Value;
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
    public async Task Handle_DeveRetornarFailure_QuandoVendaJaFechada()
    {
        var venda = CriaVendaAberta();
        venda.FecharVenda(1);
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var result = await Handle(ComandoValido());

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.FechamentoInvalido");
    }

    [Fact]
    public async Task Handle_DeveRetornarFailure_QuandoVendaCancelada()
    {
        var venda = CriaVendaAberta();
        venda.CancelarVenda();
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var result = await Handle(ComandoValido());

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.FechamentoInvalido");
    }

    [Fact]
    public async Task Handle_DeveFecharVendaEDevolverEstoque_QuandoVendaAbertaComItensEmAberto()
    {
        var venda = CriaVendaAberta();
        var estoque = Lote.Criar(1, 1, DateOnly.MaxValue, 20u).Value.Estoque;
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<int, Estoque> { { 1, estoque } });

        var result = await Handle(ComandoValido());

        result.IsSuccess.ShouldBeTrue();
        venda.Status.ShouldBe(StatusVenda.Fechada);
        venda.Itens.First().EmAberto.ShouldBe(0u);
        venda.Itens.First().Devolvido.ShouldBe(10u);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveFecharSemMovimentoEstoque_QuandoTodosItensJaResolvidos()
    {
        var venda = CriaVendaAberta();
        var item = venda.Itens.First();
        item.AdicionarHistorico(5u, 5u, 1);

        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var result = await Handle(ComandoValido());

        result.IsSuccess.ShouldBeTrue();
        venda.Status.ShouldBe(StatusVenda.Fechada);
        venda.Itens.First().EmAberto.ShouldBe(0u);
        await _estoques.DidNotReceive().GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
