using System.Net;
using System.Net.Http.Json;
using KProject.Domain.Produtos;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Lotes;

[Collection(nameof(DatabaseCollection))]
public class CriaLoteEndpointTests(DatabaseFixture fixture)
{
    private record ErrorResponse(string Code, string Description);

    private async Task<int> SeedProduto(string nome)
    {
        int id = 0;
        await fixture.ExecuteDbContext(async db =>
        {
            var produto = Produto.Criar(nome, $"REF-{nome}", "Descricao", "ANVISA-001").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();
            id = produto.Id;
        });
        return id;
    }

    [Fact]
    public async Task CriaLote_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PostAsJsonAsync("/api/lotes", new
        {
            ProdutoId = 1,
            Numero = 1,
            Validade = "2027-01-01",
            QuantidadeInicial = 0
        }, TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriaLote_Valido_DeveRetornar201ComLocation()
    {
        var client = await fixture.CriaClienteAutenticado("cria_lote_201@wilasj.dev", "Big_password!!@21");
        var produtoId = await SeedProduto("Produto CriaLoteEndpoint A");

        var result = await client.PostAsJsonAsync("/api/lotes", new
        {
            ProdutoId = produtoId,
            Numero = 1,
            Validade = "2027-06-01",
            QuantidadeInicial = 100
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);
        result.Headers.Location.ShouldNotBeNull();
        result.Headers.Location.ToString().ShouldStartWith("/api/lotes/");

        var loteId = int.Parse(result.Headers.Location.ToString().Split('/').Last());
        await fixture.ExecuteDbContext(async db =>
        {
            var lote = await db.Lotes.FindAsync(loteId);
            lote.ShouldNotBeNull();
            lote.ProdutoId.ShouldBe(produtoId);
            lote.Numero.ShouldBe(1);

            var estoque = await db.Estoques.FirstOrDefaultAsync(e => e.LoteId == loteId);
            estoque.ShouldNotBeNull();
            estoque.QuantidadeAtual.ShouldBe(100);
        });
    }

    [Fact]
    public async Task CriaLote_ProdutoNaoEncontrado_DeveRetornar404()
    {
        var client = await fixture.CriaClienteAutenticado("cria_lote_404@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/lotes", new
        {
            ProdutoId = 999999,
            Numero = 1,
            Validade = "2027-01-01",
            QuantidadeInicial = 0
        }, TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CriaLote_NumeroZero_DeveRetornar400()
    {
        var client = await fixture.CriaClienteAutenticado("cria_lote_400@wilasj.dev", "Big_password!!@21");
        var produtoId = await SeedProduto("Produto CriaLoteEndpoint B");

        var result = await client.PostAsJsonAsync("/api/lotes", new
        {
            ProdutoId = produtoId,
            Numero = 0,
            Validade = "2027-01-01",
            QuantidadeInicial = 0
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == "CriaLote.NumeroInvalido");
    }
}
