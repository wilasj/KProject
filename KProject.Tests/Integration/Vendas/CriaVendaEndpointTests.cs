using System.Net;
using System.Net.Http.Json;
using KProject.Domain.Clientes;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Vendas;

[Collection(nameof(DatabaseCollection))]
public class CriaVendaEndpointTests(DatabaseFixture fixture)
{
    private record IdResponse(int Id);

    private async Task<(int clienteId, int loteId)> SeedClienteELote(
        string suffix, uint quantidadeInicial = 20)
    {
        int clienteId = 0, loteId = 0;
        await fixture.ExecuteDbContext(async db =>
        {
            var cliente = Cliente.Criar($"Cliente {suffix}");
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();
            clienteId = cliente.Id;

            var produto = Produto.Criar($"Produto {suffix}", $"REF-{suffix}", "Desc", $"ANVISA-{suffix}").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 1, DateOnly.MaxValue, quantidadeInicial).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();
            loteId = lote.Id;
        });
        return (clienteId, loteId);
    }

    [Fact]
    public async Task CriaVenda_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PostAsJsonAsync("/api/vendas",
            new { clienteId = 1, itens = Array.Empty<object>() },
            TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriaVenda_Valida_DeveRetornar201_ComVendaEEstoqueCorretos()
    {
        var (clienteId, loteId) = await SeedClienteELote("V201");
        var client = await fixture.CriaClienteAutenticado("cria_venda_201@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/vendas", new
        {
            clienteId,
            itens = new[] { new { loteId, pacienteNome = "Joao Silva", quantidade = 3 } }
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);
        result.Headers.Location.ShouldNotBeNull();
        result.Headers.Location.ToString().ShouldStartWith("/api/vendas/");

        var response = await result.Content.ReadFromJsonAsync<IdResponse>(TestContext.Current.CancellationToken);
        response!.Id.ShouldBeGreaterThan(0);

        await fixture.ExecuteDbContext(async db =>
        {
            var venda = await db.Vendas
                .Include(v => v.Itens)
                .FirstAsync(v => v.Id == response.Id, TestContext.Current.CancellationToken);

            venda.ClienteId.ShouldBe(clienteId);
            venda.Itens.Count.ShouldBe(1);
            venda.Itens.First().PacienteNome.ShouldBe("Joao Silva");

            var estoque = await db.Estoques
                .Include(e => e.Historico)
                    .ThenInclude(h => h.Venda)
                .FirstAsync(e => e.LoteId == loteId, TestContext.Current.CancellationToken);

            estoque.QuantidadeAtual.ShouldBe(17);

            var ultimoMov = estoque.Historico.OrderByDescending(h => h.CriadoEm).First();
            ultimoMov.Tipo.ShouldBe(TipoHistorico.SaidaConsignacao);
            ultimoMov.DeltaQuantidade.ShouldBe(-3);
            ultimoMov.Venda.ShouldNotBeNull();
            ultimoMov.Venda.Id.ShouldBe(response!.Id);
        });
    }

    [Fact]
    public async Task CriaVenda_MesmoLoteDoisPacientes_DeveDecrementarSoma()
    {
        var (clienteId, loteId) = await SeedClienteELote("V2Pac");
        var client = await fixture.CriaClienteAutenticado("cria_venda_2pac@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/vendas", new
        {
            clienteId,
            itens = new[]
            {
                new { loteId, pacienteNome = "Paciente A", quantidade = 4 },
                new { loteId, pacienteNome = "Paciente B", quantidade = 6 }
            }
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);

        var response = await result.Content.ReadFromJsonAsync<IdResponse>(TestContext.Current.CancellationToken);

        await fixture.ExecuteDbContext(async db =>
        {
            var venda = await db.Vendas
                .Include(v => v.Itens)
                .FirstAsync(v => v.Id == response!.Id, TestContext.Current.CancellationToken);
            venda.Itens.Count.ShouldBe(2);

            var estoque = await db.Estoques.FirstAsync(
                e => e.LoteId == loteId, TestContext.Current.CancellationToken);
            estoque.QuantidadeAtual.ShouldBe(10);
        });
    }
}
