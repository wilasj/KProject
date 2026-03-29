using System.Net;
using System.Net.Http.Json;
using KProject.Application.Vendas.ObtemVenda;
using KProject.Domain.Clientes;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Domain.Vendas;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Vendas;

[Collection(nameof(DatabaseCollection))]
public class CancelaVendaEndpointTests(DatabaseFixture fixture)
{
    private async Task<(int vendaId, int loteId)> CriaVendaAberta(
        string suffix, uint quantidadeConsignada = 10, uint quantidadeEstoque = 20)
    {
        int vendaId = 0, loteId = 0;
        var usuarioId = await fixture.CriaUsuarioFixture($"cancela_fixture_{suffix}@wilasj.dev", "Big_password!!@21");

        await fixture.ExecuteDbContext(async db =>
        {
            var cliente = Cliente.Criar($"Cliente Cancela {suffix}");
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var produto = Produto.Criar($"Produto Cancela {suffix}", $"REF-C{suffix}", "Desc", $"ANVISA-C{suffix}").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 1, DateOnly.MaxValue, quantidadeEstoque).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();
            loteId = lote.Id;

            var venda = Venda.Criar(
                cliente.Id, usuarioId,
                new Dictionary<(int, string), uint>
                {
                    { (lote.Id, "Paciente Cancela"), quantidadeConsignada }
                }
            ).Value;
            db.Vendas.Add(venda);

            lote.Estoque.AplicarMovimento(quantidadeConsignada, TipoHistorico.SaidaConsignacao);

            await db.SaveChangesAsync();
            vendaId = venda.Id;
        });

        return (vendaId, loteId);
    }

    [Fact]
    public async Task CancelaVenda_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PostAsync("/api/vendas/1/cancel", null,
            TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CancelaVenda_DeveRetornar404_QuandoVendaNaoExiste()
    {
        var client = await fixture.CriaClienteAutenticado("cancela_404@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsync("/api/vendas/999999/cancel", null,
            TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CancelaVenda_DeveRetornar204_EDevolverTodoEstoque()
    {
        var (vendaId, loteId) = await CriaVendaAberta("204", quantidadeConsignada: 10, quantidadeEstoque: 20);
        var client = await fixture.CriaClienteAutenticado("cancela_204@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsync($"/api/vendas/{vendaId}/cancel", null,
            TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent, body);

        var getResult = await client.GetAsync($"/api/vendas/{vendaId}", TestContext.Current.CancellationToken);
        var venda = await getResult.Content.ReadFromJsonAsync<VendaDetalheResponse>(
            DatabaseFixture.JsonOptions, TestContext.Current.CancellationToken);

        venda.ShouldNotBeNull();
        venda.Status.ShouldBe(StatusVenda.Cancelada);

        await fixture.ExecuteDbContext(async db =>
        {
            var estoque = await db.Estoques
                .Include(e => e.Historico)
                .FirstAsync(e => e.LoteId == loteId, TestContext.Current.CancellationToken);

            estoque.QuantidadeAtual.ShouldBe(20);

            var retorno = estoque.Historico
                .OrderByDescending(h => h.CriadoEm)
                .First();
            retorno.Tipo.ShouldBe(TipoHistorico.RetornoConsignacao);
            retorno.DeltaQuantidade.ShouldBe(10);
        });
    }

    [Fact]
    public async Task CancelaVenda_DeveRetornar400_QuandoVendaJaCancelada()
    {
        var (vendaId, _) = await CriaVendaAberta("JaCancelada", quantidadeConsignada: 5, quantidadeEstoque: 20);
        var client = await fixture.CriaClienteAutenticado("cancela_dupla@wilasj.dev", "Big_password!!@21");

        var firstResult = await client.PostAsync($"/api/vendas/{vendaId}/cancel", null,
            TestContext.Current.CancellationToken);
        firstResult.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var secondResult = await client.PostAsync($"/api/vendas/{vendaId}/cancel", null,
            TestContext.Current.CancellationToken);
        secondResult.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelaVenda_DeveRetornar400_QuandoVendaJaFechada()
    {
        var (vendaId, _) = await CriaVendaAberta("JaFechada", quantidadeConsignada: 5, quantidadeEstoque: 20);
        var client = await fixture.CriaClienteAutenticado("cancela_fechada@wilasj.dev", "Big_password!!@21");

        var closeResult = await client.PostAsync($"/api/vendas/{vendaId}/close", null,
            TestContext.Current.CancellationToken);
        closeResult.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var cancelResult = await client.PostAsync($"/api/vendas/{vendaId}/cancel", null,
            TestContext.Current.CancellationToken);
        cancelResult.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
