using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Produto;

[Collection(nameof(DatabaseCollection))]
public class CriaProdutoEndpointTests(DatabaseFixture fixture)
{
    private record ErrorResponse(string Code, string Description);
    private record CriaProdutoResponse(int Id);

    private async Task<HttpClient> CriaClienteAutenticado(string email)
    {
        var client = fixture.Factory.CreateClient();
        var credentials = new { Email = email, Password = "Big_password!!@21" };

        await client.PostAsJsonAsync("/api/users/register", credentials, TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync("/api/users/login", credentials, TestContext.Current.CancellationToken);

        return client;
    }

    [Fact]
    public async Task CriaProduto_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PostAsJsonAsync("/api/produtos", new
        {
            Nome = "Produto Teste",
            Referencia = "REF-401",
            Descricao = "Descrição",
            CodigoAnvisa = "ANVISA-401"
        }, TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriaProduto_DeveRetornar201_SeCommandValido()
    {
        var client = await CriaClienteAutenticado("cria_produto@wilasj.dev");

        var result = await client.PostAsJsonAsync("/api/produtos", new
        {
            Nome = "Produto Teste",
            Referencia = "REF-001",
            Descricao = "Uma descrição qualquer",
            CodigoAnvisa = "ANVISA-001"
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);

        var response = await result.Content.ReadFromJsonAsync<CriaProdutoResponse>(TestContext.Current.CancellationToken);
        response!.Id.ShouldBeGreaterThan(0);

        await fixture.ExecuteDbContext(async db =>
        {
            var produto = await db.Produtos.FirstOrDefaultAsync(p => p.Referencia == "REF-001");
            produto.ShouldNotBeNull();
            produto.Nome.ShouldBe("Produto Teste");
        });
    }

    [Theory]
    [InlineData("", "REF-002", "Descrição", "ANVISA-002", "CriaProduto.NomeVazio")]
    [InlineData("Produto", "", "Descrição", "ANVISA-002", "CriaProduto.ReferenciaVazia")]
    [InlineData("Produto", "REF-002", "", "ANVISA-002", "CriaProduto.DescricaoVazia")]
    [InlineData("Produto", "REF-002", "Descrição", "", "CriaProduto.CodigoAnvisaVazio")]
    public async Task CriaProduto_DeveRetornar400_SeFieldsVazios(
        string nome, string referencia, string descricao, string codigoAnvisa, string codigoEsperado)
    {
        var client = await CriaClienteAutenticado("cria_produto_vazio@wilasj.dev");

        var result = await client.PostAsJsonAsync("/api/produtos", new
        {
            Nome = nome,
            Referencia = referencia,
            Descricao = descricao,
            CodigoAnvisa = codigoAnvisa
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == codigoEsperado);
    }

    [Theory]
    [InlineData(101, 50, 100, 50, "CriaProduto.NomeMuitoLongo")]
    [InlineData(50, 101, 100, 50, "CriaProduto.ReferenciaMuitoLonga")]
    [InlineData(50, 50, 301, 50, "CriaProduto.DescricaoMuitoLonga")]
    [InlineData(50, 50, 100, 101, "CriaProduto.CodigoAnvisaMuitoLongo")]
    public async Task CriaProduto_DeveRetornar400_SeFieldsMuitoLongos(
        int nomeLen, int referenciaLen, int descricaoLen, int codigoAnvisaLen, string codigoEsperado)
    {
        var client = await CriaClienteAutenticado("cria_produto_longo@wilasj.dev");

        var result = await client.PostAsJsonAsync("/api/produtos", new
        {
            Nome = new string('a', nomeLen),
            Referencia = new string('a', referenciaLen),
            Descricao = new string('a', descricaoLen),
            CodigoAnvisa = new string('a', codigoAnvisaLen)
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == codigoEsperado);
    }
}
