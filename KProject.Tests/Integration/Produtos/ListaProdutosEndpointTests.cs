using System.Net;
using System.Net.Http.Json;
using KProject.Application.Produtos.ListaProdutos;
using KProject.Application.Shared;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Produtos;

[Collection(nameof(DatabaseCollection))]
public class ListaProdutosEndpointTests(DatabaseFixture fixture)
{
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

    private async Task SeedLote(int produtoId, int numero)
    {
        await fixture.ExecuteDbContext(async db =>
        {
            var lote = Lote.Criar(produtoId, numero, new DateOnly(2027, 1, 1), 10).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task ListaProdutos_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.GetAsync("/api/produtos", TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListaProdutos_ProdutoSemLotes_DeveRetornarTotalLotesZero()
    {
        await SeedProduto("Produto Sem Lotes XYZ");
        var client = await fixture.CriaClienteAutenticado("lista_prod_sem_lotes@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/produtos?busca=Produto+Sem+Lotes+XYZ", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<ProdutoResponse>>(DatabaseFixture.JsonOptions, TestContext.Current.CancellationToken);
        page!.Items.ShouldHaveSingleItem();
        page.Items[0].TotalLotes.ShouldBe(0);
    }

    [Fact]
    public async Task ListaProdutos_ProdutoComLotes_DeveRetornarTotalLotesCorreto()
    {
        var produtoId = await SeedProduto("Produto Com Lotes ABC");
        await SeedLote(produtoId, 1);
        await SeedLote(produtoId, 2);

        var client = await fixture.CriaClienteAutenticado("lista_prod_com_lotes@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/produtos?busca=Produto+Com+Lotes+ABC", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<ProdutoResponse>>(DatabaseFixture.JsonOptions, TestContext.Current.CancellationToken);
        page!.Items.ShouldHaveSingleItem();
        page.Items[0].TotalLotes.ShouldBe(2);
    }
}
