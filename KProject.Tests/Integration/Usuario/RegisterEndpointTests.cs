using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Usuario;

[Collection(nameof(DatabaseCollection))]
public class RegisterEndpointTests(DatabaseFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    private record ErrorResponse(string Code, string Description);

    [Fact]
    public async Task Registrar_DeveRetornar201_SeTokenValido()
    {
        var token = await fixture.CriaInviteToken();

        var result = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "register_valido@wilasj.dev",
            Password = "Big_password!!@21",
            InviteToken = token,
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Created, body);

        await fixture.ExecuteDbContext(async db =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "register_valido@wilasj.dev");
            user.ShouldNotBeNull();
        });
    }

    [Fact]
    public async Task Registrar_DeveInvalidarToken_AposUso()
    {
        var token = await fixture.CriaInviteToken();

        await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "token_uso1@wilasj.dev",
            Password = "Big_password!!@21",
            InviteToken = token,
        }, TestContext.Current.CancellationToken);

        var result = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "token_uso2@wilasj.dev",
            Password = "Big_password!!@21",
            InviteToken = token,
        }, TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Registrar_DeveRetornar400_SeTokenInvalido()
    {
        var result = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "token_invalido@wilasj.dev",
            Password = "Big_password!!@21",
            InviteToken = "token-inexistente",
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == "Register.TokenInvalido");
    }

    [Theory]
    [InlineData("", "Big_password!!@21", "Register.EmailVazio")]
    [InlineData("nao-eh-email", "Big_password!!@21", "Register.EmailInvalido")]
    [InlineData("valido@wilasj.dev", "", "Register.SenhaVazia")]
    [InlineData("valido@wilasj.dev", "Big_password!!@21", "Register.TokenVazio")]
    public async Task Registrar_DeveRetornar400_SeCommandInvalido(string email, string password, string codigoEsperado)
    {
        var result = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = email,
            Password = password,
            InviteToken = codigoEsperado == "Register.TokenVazio" ? "" : "qualquer",
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == codigoEsperado);
    }
}
