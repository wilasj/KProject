using System.Net;
using System.Net.Http.Json;
using KProject.Application.Lotes.HistoricoLote;
using KProject.Domain.Clientes;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Domain.Vendas;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Lotes;

[Collection(nameof(DatabaseCollection))]
public class HistoricoLoteEndpointTests(DatabaseFixture fixture)
{
    private async Task<int> SeedLoteComMovimentos(string tag, int movimentos)
    {
        int loteId = 0;
        await fixture.ExecuteDbContext(async db =>
        {
            var produto = Produto.Criar($"Produto {tag}", $"REF-{tag}", "Descricao", "ANVISA-001").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            // Criar já registra um Entrada com quantidadeInicial
            var lote = Lote.Criar(produto.Id, 1, new DateOnly(2028, 1, 1), quantidadeInicial: 100).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();

            // movimentos inclui a Entrada inicial; adicionamos o restante
            for (var i = 1; i < movimentos; i++)
                lote.Estoque.AplicarMovimento(1, TipoHistorico.SaidaConsignacao);

            await db.SaveChangesAsync();
            loteId = lote.Id;
        });
        return loteId;
    }

    [Fact]
    public async Task HistoricoLote_SemAutenticacao_DeveRetornar401()
    {
        var client = fixture.Factory.CreateClient();

        var response = await client.GetAsync("/api/lotes/1/historico", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HistoricoLote_LoteInexistente_DeveRetornar404()
    {
        var client = await fixture.CriaClienteAutenticado("historico_404@wilasj.dev", "Big_password!!@21");

        var response = await client.GetAsync("/api/lotes/99999/historico", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HistoricoLote_LoteComPoucosMovimentos_DeveRetornarTodosComHasMoreFalso()
    {
        var client = await fixture.CriaClienteAutenticado("historico_hasmore_false@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("HasMoreFalse", movimentos: 5);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=1&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.Count.ShouldBe(5);
        body.HasMore.ShouldBeFalse();
    }

    [Fact]
    public async Task HistoricoLote_PrimeiraPaginaCheia_HasMoreVerdadeiro()
    {
        var client = await fixture.CriaClienteAutenticado("historico_hasmore_true@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("HasMoreTrue", movimentos: 25);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=1&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.Count.ShouldBe(20);
        body.HasMore.ShouldBeTrue();
    }

    [Fact]
    public async Task HistoricoLote_SegundaPagina_RetornaRemanescentesComHasMoreFalso()
    {
        var client = await fixture.CriaClienteAutenticado("historico_pag2@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("Pag2", movimentos: 25);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=2&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.Count.ShouldBe(5);
        body.HasMore.ShouldBeFalse();
    }

    [Fact]
    public async Task HistoricoLote_Ordem_MaisRecentePrimeiro()
    {
        var client = await fixture.CriaClienteAutenticado("historico_ordem@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("Ordem", movimentos: 3);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items
            .Zip(body.Items.Skip(1))
            .ShouldAllBe(pair => pair.First.CriadoEm >= pair.Second.CriadoEm);
    }

    [Fact]
    public async Task HistoricoLote_SemQueryParams_UsaDefaultsPagina1Tamanho20()
    {
        var client = await fixture.CriaClienteAutenticado("historico_defaults@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("Defaults", movimentos: 3);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task HistoricoLote_MovimentoComVenda_DeveRetornarVendaId()
    {
        var client = await fixture.CriaClienteAutenticado("historico_vendaid@wilasj.dev", "Big_password!!@21");
        int loteId = 0, vendaId = 0;

        await fixture.ExecuteDbContext(async db =>
        {
            var produto = Produto.Criar("Produto VendaId", "REF-VID", "Descricao", "ANVISA-VID").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 1, new DateOnly(2028, 1, 1), quantidadeInicial: 100).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();
            loteId = lote.Id;

            var clienteEntity = Cliente.Criar("Cliente VendaId");
            db.Clientes.Add(clienteEntity);
            await db.SaveChangesAsync();

            var venda = Venda.Criar(clienteEntity.Id, 1, new Dictionary<(int, string), uint>
            {
                { (lote.Id, "Paciente Teste"), 5u }
            }).Value;
            db.Vendas.Add(venda);

            lote.Estoque.AplicarMovimento(5, TipoHistorico.SaidaConsignacao, venda);
            await db.SaveChangesAsync();
            vendaId = venda.Id;
        });

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=1&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.ShouldContain(i => i.VendaId == vendaId);
    }

    [Fact]
    public async Task HistoricoLote_MovimentoSemVenda_VendaIdDeveSerNulo()
    {
        var client = await fixture.CriaClienteAutenticado("historico_nullvendaid@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("NullVendaId", movimentos: 1);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=1&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.ShouldAllBe(i => i.VendaId == null);
    }

    [Fact]
    public async Task HistoricoLote_MovimentoComCriadoPor_DeveRetornarNomeDoUsuario()
    {
        var userId = await fixture.CriaUsuarioFixture("historico_criadopor@wilasj.dev", "Big_password!!@21");
        var client = await fixture.CriaClienteAutenticado("historico_criadopor@wilasj.dev", "Big_password!!@21");
        int loteId = 0;

        await fixture.ExecuteDbContext(async db =>
        {
            var produto = Produto.Criar("Produto CriadoPor", "REF-CP", "Descricao", "ANVISA-CP").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            var lote = Lote.Criar(produto.Id, 1, new DateOnly(2028, 1, 1), quantidadeInicial: 100, criadoPor: userId).Value;
            db.Lotes.Add(lote);
            await db.SaveChangesAsync();
            loteId = lote.Id;
        });

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=1&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.ShouldContain(i => i.CriadoPor == "historico_criadopor@wilasj.dev");
    }

    [Fact]
    public async Task HistoricoLote_MovimentoSemCriadoPor_CriadoPorDeveSerNulo()
    {
        var client = await fixture.CriaClienteAutenticado("historico_nullcp@wilasj.dev", "Big_password!!@21");
        var loteId = await SeedLoteComMovimentos("NullCP", movimentos: 1);

        var response = await client.GetAsync(
            $"/api/lotes/{loteId}/historico?pagina=1&tamanhoPagina=20",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HistoricoPage>(TestContext.Current.CancellationToken);
        body!.Items.ShouldAllBe(i => i.CriadoPor == null);
    }
}
