using System.Net;
using System.Net.Http.Json;
using KProject.Application.Clientes.ListaClientes;
using KProject.Application.Shared;
using KProject.Domain.Clientes;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Clientes;

[Collection(nameof(DatabaseCollection))]
public class ListaClientesEndpointTests(DatabaseFixture fixture)
{
    private async Task CriaClienteFixture(string nome)
    {
        await fixture.ExecuteDbContext(async db =>
        {
            db.Clientes.Add(Cliente.Criar(nome));
            await db.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task ListaClientes_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.GetAsync("/api/clientes", TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListaClientes_DeveRetornar200_ComPaginaVazia()
    {
        var client = await fixture.CriaClienteAutenticado("lista_clientes_vazia@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/clientes?busca=naoexiste_xyzabc", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<ClienteResponse>>(TestContext.Current.CancellationToken);
        page!.Items.ShouldBeEmpty();
        page.Total.ShouldBe(0);
    }

    [Fact]
    public async Task ListaClientes_DeveRetornarClientes_ComAutenticacao()
    {
        await CriaClienteFixture("Drogaria Fixture");

        var client = await fixture.CriaClienteAutenticado("lista_clientes_auth@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/clientes", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<ClienteResponse>>(TestContext.Current.CancellationToken);
        page!.Total.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task ListaClientes_DeveFiltrarPorNome()
    {
        await fixture.ExecuteDbContext(async db =>
        {
            db.Clientes.Add(Cliente.Criar("Farmácia Buscavel"));
            db.Clientes.Add(Cliente.Criar("Drogaria Ignorada"));
            await db.SaveChangesAsync();
        });

        var client = await fixture.CriaClienteAutenticado("lista_clientes_busca@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/clientes?busca=farmácia+buscavel", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var page = await result.Content.ReadFromJsonAsync<Page<ClienteResponse>>(TestContext.Current.CancellationToken);
        page!.Items.ShouldAllBe(c => c.Nome.ToLower().Contains("farmácia buscavel"));
    }
}
