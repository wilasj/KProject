using KProject.Application.Produto.ListaProdutos;
using KProject.Application.Shared;
using KProject.Infrastructure.Shared;
using KProject.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using KProject.Common;

namespace KProject.Tests.Integration.Produto;

[Collection(nameof(DatabaseCollection))]
public class ListaProdutosQueryHandlerTests(DatabaseFixture fixture)
{
    private async Task<Page<ProdutoResponse>> Handle(ListaProdutosQuery query)
    {
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var handler = new ListaProdutosQueryHandler(db);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        return result.Value;
    }

    private async Task SeedProdutos(params string[] nomes)
    {
        await fixture.ExecuteDbContext(async db =>
        {
            foreach (var nome in nomes)
            {
                var produto = Domain.Produto.Produto.Criar(nome, $"REF-{nome}", "Descrição", "ANVISA-001").Value;
                db.Produtos.Add(produto);
            }
            await db.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task ListaProdutos_SemBusca_RetornaTodosOsProdutos()
    {
        await SeedProdutos("Paracetamol A", "Ibuprofeno A", "Dipirona A");

        var result = await Handle(new ListaProdutosQuery());

        result.Items.Count.ShouldBeGreaterThanOrEqualTo(3);
        result.Total.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task ListaProdutos_ComBusca_FiltraPorNome()
    {
        await SeedProdutos("Paracetamol B", "Ibuprofeno B", "Dipirona B");

        var result = await Handle(new ListaProdutosQuery { Busca = "Ibuprofeno B" });

        result.Items.ShouldContain(p => p.Nome == "Ibuprofeno B");
        result.Items.ShouldAllBe(p => p.Nome.Contains("Ibuprofeno B", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ListaProdutos_Busca_DeveSerCaseInsensitive()
    {
        await SeedProdutos("Paracetamol C");

        var result = await Handle(new ListaProdutosQuery { Busca = "PARACETAMOL C" });

        result.Items.ShouldContain(p => p.Nome == "Paracetamol C");
    }

    [Fact]
    public async Task ListaProdutos_SemMatch_RetornaVazio()
    {
        var result = await Handle(new ListaProdutosQuery { Busca = "xyzprodutoinexistente123" });

        result.Items.ShouldBeEmpty();
        result.Total.ShouldBe(0);
    }

    [Fact]
    public async Task ListaProdutos_Paginacao_RespeitaPageSize()
    {
        await SeedProdutos("Paginacao A1", "Paginacao A2", "Paginacao A3", "Paginacao A4", "Paginacao A5");

        var result = await Handle(new ListaProdutosQuery { Busca = "Paginacao A", Page = 1, PageSize = 2 });

        result.Items.Count.ShouldBe(2);
        result.Total.ShouldBeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task ListaProdutos_Paginacao_RetornaPaginaCorreta()
    {
        await SeedProdutos("Paginacao B1", "Paginacao B2", "Paginacao B3");

        var pagina1 = await Handle(new ListaProdutosQuery { Busca = "Paginacao B", Page = 1, PageSize = 2 });
        var pagina2 = await Handle(new ListaProdutosQuery { Busca = "Paginacao B", Page = 2, PageSize = 2 });

        pagina1.Items.Count.ShouldBe(2);
        pagina2.Items.Count.ShouldBe(1);
        pagina1.Items.Select(p => p.Id).Intersect(pagina2.Items.Select(p => p.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task ListaProdutos_OrdenadoPorNome()
    {
        await SeedProdutos("Ordem Z", "Ordem A", "Ordem M");

        var result = await Handle(new ListaProdutosQuery { Busca = "Ordem" });

        var nomes = result.Items.Select(p => p.Nome).ToList();
        nomes.ShouldBe(nomes.OrderBy(n => n).ToList());
    }
}
