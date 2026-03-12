using System.Net;
using System.Net.Http.Json;
using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;
using KProject.Domain.Clientes;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Domain.Vendas;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Vendas;

[Collection(nameof(DatabaseCollection))]
public class ListaVendasEndpointTests(DatabaseFixture fixture)
{
    private async Task<int> CriaVendaFixture(int usuarioId)
    {
        int vendaId = 0;

        await fixture.ExecuteDbContext(async db =>
        {
            var cliente = CriaCliente("Cliente Fixture");
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var produto = Produto.Criar("Produto Fixture", "REF-FXT", "Desc", "ANVISA-FXT").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90))).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();

            var venda = Venda.Criar(cliente.Id, usuarioId, new Dictionary<(int, string), uint> { { (lote.Id, "Paciente"), 5u } }).Value;
            db.Vendas.Add(venda);
            await db.SaveChangesAsync();

            vendaId = venda.Id;
        });

        return vendaId;
    }

    private static Cliente CriaCliente(string nome) => Cliente.Criar(nome);

    [Fact]
    public async Task ListaVendas_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.GetAsync("/api/vendas", TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListaVendas_DeveRetornar200_ComPaginaVazia()
    {
        var client = await fixture.CriaClienteAutenticado("lista_vendas_vazia@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/vendas?busca=naoexiste_xyzabc", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<VendaResponse>>(TestContext.Current.CancellationToken);
        page!.Items.ShouldBeEmpty();
        page.Total.ShouldBe(0);
    }

    [Fact]
    public async Task ListaVendas_DeveRetornarVendas_ComAutenticacao()
    {
        var usuarioId = await fixture.CriaUsuarioFixture();
        await CriaVendaFixture(usuarioId);

        var client = await fixture.CriaClienteAutenticado("lista_vendas_auth@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/vendas", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<VendaResponse>>(TestContext.Current.CancellationToken);
        page!.Total.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task ListaVendas_DeveFiltrarPorNomeDoCliente()
    {
        var usuarioId = await fixture.CriaUsuarioFixture();

        await fixture.ExecuteDbContext(async db =>
        {
            var clienteA = CriaCliente("Farmácia Central");
            var clienteB = CriaCliente("Drogaria Sul");
            db.Clientes.AddRange(clienteA, clienteB);
            await db.SaveChangesAsync();

            var produto = Produto.Criar("Produto Busca", "REF-BSC", "Desc", "ANVISA-BSC").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60))).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();

            db.Vendas.Add(Venda.Criar(clienteA.Id, usuarioId, new Dictionary<(int, string), uint> { { (lote.Id, "Paciente"), 2u } }).Value);
            db.Vendas.Add(Venda.Criar(clienteB.Id, usuarioId, new Dictionary<(int, string), uint> { { (lote.Id, "Paciente"), 3u } }).Value);
            await db.SaveChangesAsync();
        });

        var client = await fixture.CriaClienteAutenticado("lista_vendas_busca@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/vendas?busca=farmácia", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<VendaResponse>>(TestContext.Current.CancellationToken);
        page!.Items.ShouldAllBe(v => v.ClienteNome.ToLower().Contains("farmácia"));
    }
}
