using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Usuario;

[Collection(nameof(DatabaseCollection))]
public class RegisterEndpointTests(DatabaseFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    private record ErrorResponse(string Code, string Description);

    [Fact]
    public async Task Registrar_DeveRetornar201_SeCommandValido()
    {
        var result = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "wilasj@wilasj.dev",
            Password = "Big_password!!@21"
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);

        await fixture.ExecuteDbContext(async db =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "wilasj@wilasj.dev");
            user.ShouldNotBeNull();
        });
    }

    [Theory]
    [InlineData("", "Big_password!!@21", "Register.EmailVazio")]
    [InlineData("nao-eh-email", "Big_password!!@21", "Register.EmailInvalido")]
    [InlineData("valido@wilasj.dev", "", "Register.SenhaVazia")]
    public async Task Registrar_DeveRetornar400_SeCommandInvalido(string email, string password, string codigoEsperado)
    {
        var result = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = email,
            Password = password
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == codigoEsperado);
    }

    [Fact]
    public async Task Registrar_DeveRetornar400_SeEmailJaCadastrado()
    {
        var request = new { Email = "duplicado@wilasj.dev", Password = "Big_password!!@21" };

        await _client.PostAsJsonAsync("/api/users/register", request, TestContext.Current.CancellationToken);
        var result = await _client.PostAsJsonAsync("/api/users/register", request, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);
    }
}