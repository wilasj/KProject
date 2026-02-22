using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Usuario;

[Collection(nameof(DatabaseCollection))]
public class LoginEndpointTests(DatabaseFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    private record ErrorResponse(string Code, string Description);

    [Fact]
    public async Task Login_DeveRetornar200_SeCredenciaisValidas()
    {
        var request = new { Email = "login_valido@wilasj.dev", Password = "Big_password!!@21" };

        await _client.PostAsJsonAsync("/api/users/register", request, TestContext.Current.CancellationToken);
        var result = await _client.PostAsJsonAsync("/api/users/login", request, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);
    }

    [Theory]
    [InlineData("", "Big_password!!@21", "Login.EmailVazio")]
    [InlineData("nao-eh-email", "Big_password!!@21", "Login.EmailInvalido")]
    [InlineData("valido@wilasj.dev", "", "Login.SenhaVazia")]
    public async Task Login_DeveRetornar400_SeCommandInvalido(string email, string password, string codigoEsperado)
    {
        var result = await _client.PostAsJsonAsync("/api/users/login", new
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
    public async Task Login_DeveRetornar400_SeCredenciaisErradas()
    {
        await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "credenciais_erradas@wilasj.dev",
            Password = "Big_password!!@21"
        }, TestContext.Current.CancellationToken);

        var result = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Email = "credenciais_erradas@wilasj.dev",
            Password = "senha_errada!!"
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized, body);
    }
}
