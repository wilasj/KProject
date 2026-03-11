using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Clientes;

[Collection(nameof(DatabaseCollection))]
public class CriaClienteEndpointTests(DatabaseFixture fixture)
{
    private record ErrorResponse(string Code, string Description);
    private record CriaClienteResponse(int Id);

    [Fact]
    public async Task CriaCliente_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PostAsJsonAsync("/api/clientes", new { Nome = "João Silva" },
            TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriaCliente_DeveRetornar201_SeCommandValido()
    {
        var client = await fixture.CriaClienteAutenticado("cria_cliente@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/clientes", new { Nome = "Farmácia Central" },
            TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);

        var response = await result.Content.ReadFromJsonAsync<CriaClienteResponse>(TestContext.Current.CancellationToken);
        response!.Id.ShouldBeGreaterThan(0);

        await fixture.ExecuteDbContext(async db =>
        {
            var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Nome == "Farmácia Central");
            cliente.ShouldNotBeNull();
        });
    }

    [Fact]
    public async Task CriaCliente_DeveRetornar400_SeNomeVazio()
    {
        var client = await fixture.CriaClienteAutenticado("cria_cliente_vazio@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/clientes", new { Nome = "" },
            TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == "CriaCliente.NomeVazio");
    }

    [Fact]
    public async Task CriaCliente_DeveRetornar400_SeNomeMuitoLongo()
    {
        var client = await fixture.CriaClienteAutenticado("cria_cliente_longo@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/clientes", new { Nome = new string('a', 201) },
            TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == "CriaCliente.NomeMuitoLongo");
    }
}
