using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Vendas;
using Shouldly;

namespace KProject.Tests.Unit.Estoques;

public class EstoqueTests
{
    private static Estoque CriaEstoque() =>
        Lote.Criar(1, 1, new DateOnly(2027, 1, 1)).Value.Estoque;

    [Fact]
    public void AplicarMovimento_ComQuantidadeZero_RetornaErro()
    {
        var estoque = CriaEstoque();

        var result = estoque.AplicarMovimento(0, TipoHistorico.Entrada);

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Estoque.QuantidadeInvalida");
    }

    [Fact]
    public void AplicarMovimento_SaidaSemEstoque_RetornaErro()
    {
        var estoque = CriaEstoque();

        var result = estoque.AplicarMovimento(1, TipoHistorico.Perda);

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Code.ShouldBe("Estoque.EstoqueInsuficiente");
    }

    [Theory]
    [InlineData(TipoHistorico.Entrada)]
    [InlineData(TipoHistorico.RetornoConsignacao)]
    [InlineData(TipoHistorico.AjusteEntrada)]
    public void AplicarMovimento_TiposPositivos_AumentamQuantidade(TipoHistorico tipo)
    {
        var estoque = CriaEstoque();

        estoque.AplicarMovimento(10, tipo);

        estoque.QuantidadeAtual.ShouldBe(10);
    }

    [Theory]
    [InlineData(TipoHistorico.SaidaConsignacao)]
    [InlineData(TipoHistorico.AjusteSaida)]
    [InlineData(TipoHistorico.Perda)]
    public void AplicarMovimento_TiposNegativos_DiminuemQuantidade(TipoHistorico tipo)
    {
        var estoque = CriaEstoque();
        estoque.AplicarMovimento(10, TipoHistorico.Entrada);

        estoque.AplicarMovimento(3, tipo);

        estoque.QuantidadeAtual.ShouldBe(7);
    }

    [Fact]
    public void AplicarMovimento_AdicionaHistoricoComDeltaCorreto()
    {
        var estoque = CriaEstoque();

        estoque.AplicarMovimento(10, TipoHistorico.Entrada);

        estoque.Historico.Count.ShouldBe(1);
        estoque.Historico.First().DeltaQuantidade.ShouldBe(10);
        estoque.Historico.First().Tipo.ShouldBe(TipoHistorico.Entrada);
    }

    [Fact]
    public void AplicarMovimento_MultiplaMovimentacoes_AcumulaCorretamente()
    {
        var estoque = CriaEstoque();

        estoque.AplicarMovimento(20, TipoHistorico.Entrada);
        estoque.AplicarMovimento(5, TipoHistorico.SaidaConsignacao);
        estoque.AplicarMovimento(3, TipoHistorico.RetornoConsignacao);

        estoque.QuantidadeAtual.ShouldBe(18);
        estoque.Historico.Count.ShouldBe(3);
    }

    [Fact]
    public void AplicarMovimento_SaidaExataDoEstoque_Sucesso()
    {
        var estoque = CriaEstoque();

        estoque.AplicarMovimento(5, TipoHistorico.Entrada);
        var result = estoque.AplicarMovimento(5, TipoHistorico.Perda);

        result.IsSuccess.ShouldBeTrue();
        estoque.QuantidadeAtual.ShouldBe(0);
    }

    [Fact]
    public void AplicarMovimento_ComVenda_RegistraVendaNoHistorico()
    {
        var estoque = CriaEstoque();
        estoque.AplicarMovimento(10, TipoHistorico.Entrada);
        var venda = Venda.Criar(1, 1, new Dictionary<(int, string), uint>
        {
            { (1, "Paciente"), 1u }
        }).Value;

        estoque.AplicarMovimento(1, TipoHistorico.SaidaConsignacao, venda);

        estoque.Historico.Last().Venda.ShouldBe(venda);
    }

    [Fact]
    public void AplicarMovimento_SemVenda_VendaNulaNoHistorico()
    {
        var estoque = CriaEstoque();

        estoque.AplicarMovimento(5, TipoHistorico.Entrada);

        estoque.Historico.Last().Venda.ShouldBeNull();
    }
}
