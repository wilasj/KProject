using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Clientes;
using KProject.Application.Interfaces.Estoques;
using KProject.Application.Interfaces.Vendas;
using KProject.Application.Vendas.CriaVenda;
using KProject.Common;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Vendas;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class CriaVendaCommandHandlerTests
{
    private readonly IClienteRepository _clientes = Substitute.For<IClienteRepository>();
    private readonly IEstoqueRepository _estoques = Substitute.For<IEstoqueRepository>();
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Task<Result<int>> Handle(CriaVendaCommand command) =>
        new CriaVendaCommandHandler(_clientes, _estoques, _vendas, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    private static CriaVendaCommand ComandoValido(int loteId = 1) => new()
    {
        ClienteId = 1,
        CriadaPor = 1,
        Itens = [new NovoItemDto(loteId, "Paciente Teste", 2u)]
    };

    private static Dictionary<int, Estoque> EstoquePara(int loteId, uint quantidade) =>
        new() { [loteId] = Lote.Criar(1, 1, DateOnly.MaxValue, quantidade).Value.Estoque };

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoClienteNaoExiste()
    {
        _clientes.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(false);

        var result = await Handle(ComandoValido());

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.ClienteNaoEncontrado");
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoLoteNaoEncontrado()
    {
        _clientes.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<int, Estoque>());

        var result = await Handle(ComandoValido());

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Venda.LoteNaoEncontrado");
    }

    [Fact]
    public async Task Handle_DeveRetornarFailure_QuandoEstoqueInsuficiente()
    {
        _clientes.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(EstoquePara(1, 1u));

        var command = new CriaVendaCommand
        {
            ClienteId = 1,
            CriadaPor = 1,
            Itens = [new NovoItemDto(1, "Paciente", 5u)]
        };

        var result = await Handle(command);

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Estoque.EstoqueInsuficiente");
    }

    [Fact]
    public async Task Handle_DeveSalvarVendaERetornarId_QuandoCriacaoValida()
    {
        _clientes.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(EstoquePara(1, 10u));

        var result = await Handle(ComandoValido());

        result.IsSuccess.ShouldBeTrue();
        await _vendas.Received(1).AddAsync(Arg.Any<Venda>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveAssociarVendaAoHistoricoDeEstoque()
    {
        _clientes.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        var estoques = EstoquePara(1, 10u);
        _estoques.GetByLoteIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(estoques);

        Venda? vendaSalva = null;
        await _vendas.AddAsync(Arg.Do<Venda>(v => vendaSalva = v), Arg.Any<CancellationToken>());

        await Handle(ComandoValido());

        var ultimoMov = estoques[1].Historico.Last();
        ultimoMov.Venda.ShouldNotBeNull();
        ultimoMov.Venda.ShouldBe(vendaSalva);
        ultimoMov.CriadoPor.ShouldBe(1);
    }
}
