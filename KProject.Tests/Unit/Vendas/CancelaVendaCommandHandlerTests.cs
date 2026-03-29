using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Estoques;
using KProject.Application.Interfaces.Vendas;
using KProject.Application.Vendas.CancelaVenda;
using KProject.Common;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Vendas;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class CancelaVendaCommandHandlerTests
{
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();
    private readonly IEstoqueRepository _estoques = Substitute.For<IEstoqueRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Task<Result> Handle(CancelaVendaCommand command) =>
        new CancelaVendaCommandHandler(_vendas, _estoques, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    private static CancelaVendaCommand ComandoValido(int vendaId = 1) => new()
    {
        VendaId = vendaId,
        CanceladoPor = 1
    };

    private static Venda CriaVendaAberta(uint quantidade = 10u)
    {
        return Venda.Criar(1, 1, new Dictionary<(int, string), uint>
        {
            { (1, "Paciente Teste"), quantidade }
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
        result.Errors.First().Code.ShouldBe("Venda.CancelamentoInvalido");
    }

    [Fact]
    public async Task Handle_DeveRetornarFailure_QuandoVendaJaCancelada()
    {
        var venda = CriaVendaAberta();
        venda.CancelarVenda(1);
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);

        var result = await Handle(ComandoValido());

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.CancelamentoInvalido");
    }

    [Fact]
    public async Task Handle_DeveCancelarVendaEDevolverTodoEstoque()
    {
        var venda = CriaVendaAberta(quantidade: 10u);
        var estoque = Lote.Criar(1, 1, DateOnly.MaxValue, 20u).Value.Estoque;
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<int, Estoque> { { 1, estoque } });

        var result = await Handle(ComandoValido());

        result.IsSuccess.ShouldBeTrue();
        venda.Status.ShouldBe(StatusVenda.Cancelada);
        estoque.QuantidadeAtual.ShouldBe(30);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveDevolverQuantidadeConsignadaTotal_MesmoComItensParicialmenteResolvidos()
    {
        var venda = CriaVendaAberta(quantidade: 10u);
        var item = venda.Itens.First();
        item.AdicionarHistorico(2u, 3u, 1);

        var estoque = Lote.Criar(1, 1, DateOnly.MaxValue, 20u).Value.Estoque;
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<int, Estoque> { { 1, estoque } });

        var result = await Handle(ComandoValido());

        result.IsSuccess.ShouldBeTrue();
        venda.Status.ShouldBe(StatusVenda.Cancelada);
        estoque.QuantidadeAtual.ShouldBe(30);
    }

    [Fact]
    public async Task Handle_DeveAssociarVendaAoHistoricoDeEstoque()
    {
        var venda = CriaVendaAberta(quantidade: 10u);
        var estoque = Lote.Criar(1, 1, DateOnly.MaxValue, 20u).Value.Estoque;
        _vendas.GetByIdWithItensAsync(1, Arg.Any<CancellationToken>()).Returns(venda);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<int, Estoque> { { 1, estoque } });

        await Handle(ComandoValido());

        var retorno = estoque.Historico.Last();
        retorno.Tipo.ShouldBe(TipoHistorico.RetornoConsignacao);
        retorno.Venda.ShouldBe(venda);
    }
}
