using KProject.Common;
using KProject.Domain.Venda;
using Shouldly;

namespace KProject.Tests.Venda;

public class VendaTests
{
    [Fact]
    public void Venda_NaoPodeSerCriada_SemItens()
    {
        var dict = new Dictionary<int, uint>();

        var venda = Domain.Venda.Venda.Criar(1, 1, dict);
        
        venda.Error.ShouldBe(Error.Failure("Venda.ItensInvalidos", "Nenhum item foi fornecido para a venda"));
        venda.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Venda_NaoPodeSerCriada_ComClienteInvalido()
    {
        var dict = new Dictionary<int, uint>
        {
            { 1, 1 }
        };
        
        var venda = Domain.Venda.Venda.Criar(0, 1, dict);
        
        venda.Error.ShouldBe(Error.Failure("Venda.ClienteInvalido", "O ID do cliente deve ser maior que zero"));
        venda.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Venda_NaoPodeAdicionar_ComItemDuplicado()
    {
        var dict = new Dictionary<int, uint>
        {
            { 1, 1 },
        };
        
        var venda = Domain.Venda.Venda.Criar(1, 1, dict);
        
        venda.IsSuccess.ShouldBeTrue();
        
        var item =  ItemConsignado.Criar(1, 1, 1);
        
        item.IsSuccess.ShouldBeTrue();

        var result = venda.Value.AdicionarItem(item.Value);
        
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeOfType<Error>();
        result.Error.Code.ShouldBe("Venda.ItemDuplicado");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    public void Venda_NaoPodeSerCriada_ComItensInvalidos(int loteId, uint quantidadeConsignada)
    {
        var itens = new Dictionary<int, uint>
        {
            { loteId, quantidadeConsignada }
        };

        var venda = Domain.Venda.Venda.Criar(1, 1, itens);
        
        venda.IsSuccess.ShouldBeFalse();
        venda.Error.ShouldBeOfType<Error>();
    }

    [Theory]
    [InlineData(StatusVenda.Fechada)]
    [InlineData(StatusVenda.Cancelada)]
    public void Venda_NaoPodeAdicionarItem_ComStatusInvalidos(StatusVenda status)
    {
        var itens = new Dictionary<int, uint>
        {
            { 1, 1 }
        };
        
        var venda = Domain.Venda.Venda.Criar(1, 1, itens);

        venda.Value.ShouldBeOfType<Domain.Venda.Venda>();
        venda.Error.ShouldBe(Error.None);

        if (status is StatusVenda.Fechada)
        {
            venda.Value.FecharVenda();
        }
        else
        {
            venda.Value.CancelarVenda();
        }

        var item = ItemConsignado.Criar(1, 1, 1).Value;

        var result = venda.Value.AdicionarItem(item);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe("Venda.StatusInvalido");
    }

    [Theory]
    [InlineData(StatusVenda.Fechada)]
    [InlineData(StatusVenda.Cancelada)]
    public void Venda_NaoPodeSerFechadaOuCancelada_ComStatusInvalidos(StatusVenda status)
    {
        var itens = new Dictionary<int, uint>
        {
            { 1, 1 }
        };

        var venda = Domain.Venda.Venda.Criar(1, 1, itens);

        venda.Value.ShouldBeOfType<Domain.Venda.Venda>();
        venda.Error.ShouldBe(Error.None);

        Result result;

        if (status is StatusVenda.Fechada)
        {
            //primeira chamada fecha, segunda chamada emite o erro
            venda.Value.FecharVenda();
            result = venda.Value.FecharVenda();
        }
        else
        {
            venda.Value.CancelarVenda();
            result = venda.Value.CancelarVenda();
        }

        result.Error.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Venda_ComItensValidos_EhCriada()
    {
        var itens = new Dictionary<int, uint>
        {
            { 1, 1 }
        };
        
        var venda = Domain.Venda.Venda.Criar(1, 1, itens);
        
        venda.IsSuccess.ShouldBeTrue();
        venda.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComUsuarioInvalido()
    {
        var item = ItemConsignado.Criar(1, 0, 1);
        
        item.IsSuccess.ShouldBeFalse();
        item.Error.ShouldBeOfType<Error>();
        item.Error.Code.ShouldBe("ItemConsignado.UsuarioInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComQuantidadeInvalida()
    {
        var item = ItemConsignado.Criar(1, 1, 0);
        
        item.IsSuccess.ShouldBeFalse();
        item.Error.ShouldBeOfType<Error>();
        item.Error.Code.ShouldBe("ItemConsignado.QuantidadeInvalida");
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComLoteInvalido()
    {
        var item = ItemConsignado.Criar(0, 1, 1);

        item.IsSuccess.ShouldBeFalse();
        item.Error.ShouldBeOfType<Error>();
        item.Error.Code.ShouldBe("ItemConsignado.LoteInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeAdicionarHistorico_ComUsuarioInvalido()
    {
        var item = ItemConsignado.Criar(1, 1, 2);

        item.IsSuccess.ShouldBeTrue();
        item.Error.ShouldBe(Error.None);

        var result = item.Value.AdicionarHistorico(1, 1, 0);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeOfType<Error>();
        result.Error.Code.ShouldBe("ItemConsignado.UsuarioInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeAdicionarHistorico_ComHistoricoInvalido()
    {
        var item = ItemConsignado.Criar(1, 1, 2);
        
        item.IsSuccess.ShouldBeTrue();
        item.Error.ShouldBe(Error.None);

        var result = item.Value.AdicionarHistorico(10, 1, 1);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeOfType<Error>();
        result.Error.Code.ShouldBe("ItemConsignado.HistoricoInvalido");
    }

    [Fact]
    public void ItemConsignado_DeveSempreRetornar_UltimoHistoricoVendido()
    {
        var item = ItemConsignado.Criar(1, 1, 2);
        
        item.IsSuccess.ShouldBeTrue();
        item.Error.ShouldBe(Error.None);

        item.Value.AdicionarHistorico(1, 1, 1);
        
        var result = item.Value.AdicionarHistorico(0, 2, 1);
        
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        item.Value.Devolvido.ShouldBe<uint>(0);
        item.Value.Vendido.ShouldBe<uint>(2);
    }

    [Fact]
    public void ItemConsignado_DeveSempreRetornar_UltimoHistoricoDevolvido()
    {
        var item = ItemConsignado.Criar(1, 1, 2);

        item.IsSuccess.ShouldBeTrue();
        item.Error.ShouldBe(Error.None);

        item.Value.AdicionarHistorico(1, 1, 1);

        var result = item.Value.AdicionarHistorico(2, 0, 1);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        item.Value.Vendido.ShouldBe<uint>(0);
        item.Value.Devolvido.ShouldBe<uint>(2);
        
    }

    [Fact]
    public void ItemConsignado_DeveSempreRetornar_EmAberto()
    {
        var item = ItemConsignado.Criar(1, 1, 5);
        
        item.IsSuccess.ShouldBeTrue();
        item.Error.ShouldBe(Error.None);

        item.Value.AdicionarHistorico(2, 1, 1);
        
        var result = item.Value.AdicionarHistorico(1, 1, 1);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        item.Value.EmAberto.ShouldBe<uint>(3);
    }
}