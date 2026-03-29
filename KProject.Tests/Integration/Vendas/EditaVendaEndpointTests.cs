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
public class EditaVendaEndpointTests(DatabaseFixture fixture)
{
    private async Task<(int vendaId, int itemId1, int itemId2)> CriaVendaComDoisItens(int usuarioId)
    {
        int vendaId = 0, itemId1 = 0, itemId2 = 0;

        await fixture.ExecuteDbContext(async db =>
        {
            var cliente = Cliente.Criar("Cliente Edita");
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var produto = Produto.Criar("Produto Edita", "REF-EDIT", "Desc", "ANVISA-EDIT").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 100, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90))).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();

            var venda = Venda.Criar(
                cliente.Id, usuarioId,
                new Dictionary<(int, string), uint>
                {
                    { (lote.Id, "Paciente A"), 10u },
                    { (lote.Id, "Paciente B"), 5u }
                }
            ).Value;
            db.Vendas.Add(venda);
            await db.SaveChangesAsync();

            vendaId = venda.Id;
            var itens = venda.Itens.OrderBy(i => i.PacienteNome).ToList();
            itemId1 = itens[0].Id;
            itemId2 = itens[1].Id;
        });

        return (vendaId, itemId1, itemId2);
    }

    [Fact]
    public async Task EditaVenda_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PatchAsJsonAsync("/api/vendas/1",
            new { itens = Array.Empty<object>() },
            cancellationToken: TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task EditaVenda_DeveRetornar404_QuandoVendaNaoExiste()
    {
        var client = await fixture.CriaClienteAutenticado("edita_venda_404@wilasj.dev", "Big_password!!@21");

        var result = await client.PatchAsJsonAsync("/api/vendas/999999",
            new { itens = new[] { new { id = 1, vendido = 1, devolvido = 0 } } },
            cancellationToken: TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditaVenda_DeveRetornar204_ComDadosValidos()
    {
        var usuarioId = await fixture.CriaUsuarioFixture("edita_fixture@wilasj.dev", "Big_password!!@21");
        var (vendaId, itemId1, itemId2) = await CriaVendaComDoisItens(usuarioId);

        var client = await fixture.CriaClienteAutenticado("edita_venda_204@wilasj.dev", "Big_password!!@21");

        var result = await client.PatchAsJsonAsync($"/api/vendas/{vendaId}", new
        {
            itens = new[]
            {
                new { id = itemId1, vendido = 3u, devolvido = 2u },
                new { id = itemId2, vendido = 1u, devolvido = 0u }
            }
        }, cancellationToken: TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent, body);

        var getResult = await client.GetAsync($"/api/vendas/{vendaId}", TestContext.Current.CancellationToken);
        var venda = await getResult.Content.ReadFromJsonAsync<VendaDetalheResponse>(
            DatabaseFixture.JsonOptions, TestContext.Current.CancellationToken);

        venda.ShouldNotBeNull();
        venda.TotalVendido.ShouldBe(4u);
        venda.TotalDevolvido.ShouldBe(2u);

        var item1 = venda.Itens.First(i => i.Id == itemId1);
        item1.Vendido.ShouldBe(3u);
        item1.Devolvido.ShouldBe(2u);
        item1.EmAberto.ShouldBe(5u);

        var item2 = venda.Itens.First(i => i.Id == itemId2);
        item2.Vendido.ShouldBe(1u);
        item2.Devolvido.ShouldBe(0u);
        item2.EmAberto.ShouldBe(4u);
    }

    [Fact]
    public async Task EditaVenda_DeveRetornar400_QuandoVendaFechada()
    {
        var usuarioId = await fixture.CriaUsuarioFixture("edita_fechada_fixture@wilasj.dev", "Big_password!!@21");
        var (vendaId, itemId1, _) = await CriaVendaComDoisItens(usuarioId);

        await fixture.ExecuteDbContext(async db =>
        {
            var venda = await db.Vendas.FindAsync(vendaId);
            venda!.FecharVenda(1);
            await db.SaveChangesAsync();
        });

        var client = await fixture.CriaClienteAutenticado("edita_venda_fechada@wilasj.dev", "Big_password!!@21");

        var result = await client.PatchAsJsonAsync($"/api/vendas/{vendaId}", new
        {
            itens = new[] { new { id = itemId1, vendido = 1u, devolvido = 0u } }
        }, cancellationToken: TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
