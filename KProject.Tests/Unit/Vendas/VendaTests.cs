using KProject.Common;
using KProject.Domain.Vendas;
using Shouldly;

namespace KProject.Tests.Unit.Vendas;

public class VendaTests
{
    [Fact]
    public void Venda_NaoPodeSerCriada_SemItens()
    {
        var dict = new Dictionary<(int LoteId, string PacienteNome), uint>();

        var venda = Venda.Criar(1, 1, dict);

        venda.Errors.First().ShouldBe(Error.Failure("Venda.ItensInvalidos", "Nenhum item foi fornecido para a venda"));
        venda.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Venda_NaoPodeSerCriada_ComClienteInvalido()
    {
        var dict = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (1, "Paciente"), 1u }
        };

        var venda = Venda.Criar(0, 1, dict);

        venda.Errors.First().ShouldBe(Error.Failure("Venda.ClienteInvalido", "O ID do cliente deve ser maior que zero"));
        venda.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Venda_NaoPodeAdicionar_ComItemDuplicado()
    {
        var dict = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (1, "Paciente"), 1u }
        };

        var venda = Venda.Criar(1, 1, dict);

        venda.IsSuccess.ShouldBeTrue();

        var item = ItemConsignado.Criar(1, 1, "Paciente", 1);

        item.IsSuccess.ShouldBeTrue();

        var result = venda.Value.AdicionarItem(item.Value);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.First().ShouldBeOfType<Error>();
        result.Errors.First().Code.ShouldBe("Venda.ItemDuplicado");
    }

    [Fact]
    public void Venda_PodeAdicionar_MesmoLote_ComPacienteDiferente()
    {
        var dict = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (1, "Paciente A"), 1u }
        };

        var venda = Venda.Criar(1, 1, dict);

        venda.IsSuccess.ShouldBeTrue();

        var item = ItemConsignado.Criar(1, 1, "Paciente B", 1);

        item.IsSuccess.ShouldBeTrue();

        var result = venda.Value.AdicionarItem(item.Value);

        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    public void Venda_NaoPodeSerCriada_ComItensInvalidos(int loteId, uint quantidadeConsignada)
    {
        var itens = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (loteId, "Paciente"), quantidadeConsignada }
        };

        var venda = Venda.Criar(1, 1, itens);

        venda.IsSuccess.ShouldBeFalse();
        venda.Errors.First().ShouldBeOfType<Error>();
    }

    [Theory]
    [InlineData(StatusVenda.Fechada)]
    [InlineData(StatusVenda.Cancelada)]
    public void Venda_NaoPodeAdicionarItem_ComStatusInvalidos(StatusVenda status)
    {
        var itens = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (1, "Paciente"), 1u }
        };

        var venda = Venda.Criar(1, 1, itens);

        venda.Value.ShouldBeOfType<Venda>();
        venda.Errors.ShouldBeEmpty();

        if (status is StatusVenda.Fechada)
            venda.Value.FecharVenda();
        else
            venda.Value.CancelarVenda();

        var item = ItemConsignado.Criar(1, 1, "Paciente", 1).Value;

        var result = venda.Value.AdicionarItem(item);

        result.Errors.First().ShouldNotBeNull();
        result.Errors.First().Code.ShouldBe("Venda.StatusInvalido");
    }

    [Theory]
    [InlineData(StatusVenda.Fechada)]
    [InlineData(StatusVenda.Cancelada)]
    public void Venda_NaoPodeSerFechadaOuCancelada_ComStatusInvalidos(StatusVenda status)
    {
        var itens = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (1, "Paciente"), 1u }
        };

        var venda = Venda.Criar(1, 1, itens);

        venda.Value.ShouldBeOfType<Venda>();
        venda.Errors.ShouldBeEmpty();

        Result result;

        if (status is StatusVenda.Fechada)
        {
            venda.Value.FecharVenda();
            result = venda.Value.FecharVenda();
        }
        else
        {
            venda.Value.CancelarVenda();
            result = venda.Value.CancelarVenda();
        }

        result.Errors.First().ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Venda_ComItensValidos_EhCriada()
    {
        var itens = new Dictionary<(int LoteId, string PacienteNome), uint>
        {
            { (1, "Paciente"), 1u }
        };

        var venda = Venda.Criar(1, 1, itens);

        venda.IsSuccess.ShouldBeTrue();
        venda.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComUsuarioInvalido()
    {
        var item = ItemConsignado.Criar(1, 0, "Paciente", 1);

        item.IsSuccess.ShouldBeFalse();
        item.Errors.First().ShouldBeOfType<Error>();
        item.Errors.First().Code.ShouldBe("ItemConsignado.UsuarioInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComQuantidadeInvalida()
    {
        var item = ItemConsignado.Criar(1, 1, "Paciente", 0);

        item.IsSuccess.ShouldBeFalse();
        item.Errors.First().ShouldBeOfType<Error>();
        item.Errors.First().Code.ShouldBe("ItemConsignado.QuantidadeInvalida");
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComLoteInvalido()
    {
        var item = ItemConsignado.Criar(0, 1, "Paciente", 1);

        item.IsSuccess.ShouldBeFalse();
        item.Errors.First().ShouldBeOfType<Error>();
        item.Errors.First().Code.ShouldBe("ItemConsignado.LoteInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeSerCriado_ComPacienteInvalido()
    {
        var item = ItemConsignado.Criar(1, 1, "", 1);

        item.IsSuccess.ShouldBeFalse();
        item.Errors.First().ShouldBeOfType<Error>();
        item.Errors.First().Code.ShouldBe("ItemConsignado.PacienteInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeAdicionarHistorico_ComUsuarioInvalido()
    {
        var item = ItemConsignado.Criar(1, 1, "Paciente", 2);

        item.IsSuccess.ShouldBeTrue();
        item.Errors.ShouldBeEmpty();

        var result = item.Value.AdicionarHistorico(1, 1, 0);
        result.IsSuccess.ShouldBeFalse();
        result.Errors.First().ShouldBeOfType<Error>();
        result.Errors.First().Code.ShouldBe("ItemConsignado.UsuarioInvalido");
    }

    [Fact]
    public void ItemConsignado_NaoPodeAdicionarHistorico_ComHistoricoInvalido()
    {
        var item = ItemConsignado.Criar(1, 1, "Paciente", 2);

        item.IsSuccess.ShouldBeTrue();
        item.Errors.ShouldBeEmpty();

        var result = item.Value.AdicionarHistorico(10, 1, 1);
        result.IsSuccess.ShouldBeFalse();
        result.Errors.First().ShouldBeOfType<Error>();
        result.Errors.First().Code.ShouldBe("ItemConsignado.HistoricoInvalido");
    }

    [Fact]
    public void ItemConsignado_DeveSempreRetornar_UltimoHistoricoVendido()
    {
        var item = ItemConsignado.Criar(1, 1, "Paciente", 2);

        item.IsSuccess.ShouldBeTrue();
        item.Errors.ShouldBeEmpty();

        item.Value.AdicionarHistorico(1, 1, 1);

        var result = item.Value.AdicionarHistorico(0, 2, 1);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        item.Value.Devolvido.ShouldBe<uint>(0);
        item.Value.Vendido.ShouldBe<uint>(2);
    }

    [Fact]
    public void ItemConsignado_DeveSempreRetornar_UltimoHistoricoDevolvido()
    {
        var item = ItemConsignado.Criar(1, 1, "Paciente", 2);

        item.IsSuccess.ShouldBeTrue();
        item.Errors.ShouldBeEmpty();

        item.Value.AdicionarHistorico(1, 1, 1);

        var result = item.Value.AdicionarHistorico(2, 0, 1);
        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        item.Value.Vendido.ShouldBe<uint>(0);
        item.Value.Devolvido.ShouldBe<uint>(2);
    }

    [Fact]
    public void ItemConsignado_DeveSempreRetornar_EmAberto()
    {
        var item = ItemConsignado.Criar(1, 1, "Paciente", 5);

        item.IsSuccess.ShouldBeTrue();
        item.Errors.ShouldBeEmpty();

        item.Value.AdicionarHistorico(2, 1, 1);

        var result = item.Value.AdicionarHistorico(1, 1, 1);
        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();

        item.Value.EmAberto.ShouldBe<uint>(3);
    }
}
