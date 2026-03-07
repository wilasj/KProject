using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Usuarios;

[Collection(nameof(DatabaseCollection))]
public class LoginEndpointTests(DatabaseFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    private record ErrorResponse(string Code, string Description);

    [Fact]
    public async Task Login_DeveRetornar200_SeCredenciaisValidas()
    {
        var email = "login_valido@wilasj.dev";
        var password = "Big_password!!@21";
        var token = await fixture.CriaConviteToken();
        await _client.PostAsJsonAsync("/api/users/register", new { Email = email, Password = password, ConviteToken = token }, TestContext.Current.CancellationToken);
        var result = await _client.PostAsJsonAsync("/api/users/login", new { Email = email, Password = password }, TestContext.Current.CancellationToken);

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
    public async Task Login_DeveRetornar401_SeCredenciaisErradas()
    {
        var token = await fixture.CriaConviteToken();
        await _client.PostAsJsonAsync("/api/users/register", new
        {
            Email = "credenciais_erradas@wilasj.dev",
            Password = "Big_password!!@21",
            InviteToken = token,
        }, TestContext.Current.CancellationToken);

        var result = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Email = "credenciais_erradas@wilasj.dev",
            Password = "senha_errada!!"
        }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized, body);

        var errors = await result.Content.ReadFromJsonAsync<List<ErrorResponse>>(TestContext.Current.CancellationToken);
        errors!.ShouldContain(e => e.Code == "Usuario.LoginFalhou");
    }
}
