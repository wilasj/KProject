using System.Net;
using System.Net.Http.Json;
using KProject.Application.Vendas.ObtemVenda;
using KProject.Domain.Clientes;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Domain.Vendas;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Vendas;

[Collection(nameof(DatabaseCollection))]
public class ObtemVendaEndpointTests(DatabaseFixture fixture)
{

    private async Task<(int vendaId, int loteNumero, string produtoNome, string pacienteNome, uint quantidade)>
        CriaVendaFixture(int usuarioId)
    {
        int vendaId = 0;
        int loteNumero = 0;
        string produtoNome = string.Empty;
        const string pacienteNome = "Paciente Teste";
        const uint quantidade = 5u;

        await fixture.ExecuteDbContext(async db =>
        {
            var cliente = Cliente.Criar("Cliente Detalhe");
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var produto = Produto.Criar("Produto Detalhe", "REF-DET", "Desc", "ANVISA-DET").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 42, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90))).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();

            var venda = Venda.Criar(
                cliente.Id, usuarioId,
                new Dictionary<(int, string), uint> { { (lote.Id, pacienteNome), quantidade } }
            ).Value;
            db.Vendas.Add(venda);
            await db.SaveChangesAsync();

            vendaId = venda.Id;
            loteNumero = lote.Numero;
            produtoNome = produto.Nome;
        });

        return (vendaId, loteNumero, produtoNome, pacienteNome, quantidade);
    }

    [Fact]
    public async Task ObtemVenda_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.GetAsync("/api/vendas/1", TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ObtemVenda_DeveRetornar404_QuandoVendaNaoExiste()
    {
        var client = await fixture.CriaClienteAutenticado("obtem_venda_404@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/vendas/999999", TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ObtemVenda_DeveRetornar200_ComDadosCorretos()
    {
        var usuarioId = await fixture.CriaUsuarioFixture();
        var (vendaId, loteNumero, produtoNome, pacienteNome, quantidade) = await CriaVendaFixture(usuarioId);

        var client = await fixture.CriaClienteAutenticado("obtem_venda_ok@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync($"/api/vendas/{vendaId}", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var venda = await result.Content.ReadFromJsonAsync<VendaDetalheResponse>(DatabaseFixture.JsonOptions, TestContext.Current.CancellationToken);
        venda.ShouldNotBeNull();
        venda.Id.ShouldBe(vendaId);
        venda.Status.ShouldBe(StatusVenda.Aberta);
        venda.ClienteNome.ShouldBe("Cliente Detalhe");
        venda.TotalConsignado.ShouldBe(quantidade);
        venda.TotalVendido.ShouldBe(0u);
        venda.TotalDevolvido.ShouldBe(0u);
        venda.Itens.Count.ShouldBe(1);

        var item = venda.Itens[0];
        item.ProdutoNome.ShouldBe(produtoNome);
        item.LoteNumero.ShouldBe(loteNumero);
        item.PacienteNome.ShouldBe(pacienteNome);
        item.QuantidadeConsignada.ShouldBe(quantidade);
        item.Vendido.ShouldBe(0u);
        item.Devolvido.ShouldBe(0u);
        item.EmAberto.ShouldBe(quantidade);
    }
}
