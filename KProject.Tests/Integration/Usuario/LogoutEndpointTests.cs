using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Usuario;

[Collection(nameof(DatabaseCollection))]
public class LogoutEndpointTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Logout_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.PostAsync("/api/users/logout", null, TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_DeveRetornar200_ComAutenticacao()
    {
        var client = fixture.Factory.CreateClient();
        var credentials = new { Email = "logout_valido@wilasj.dev", Password = "Big_password!!@21" };

        await client.PostAsJsonAsync("/api/users/register", credentials, TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync("/api/users/login", credentials, TestContext.Current.CancellationToken);

        var result = await client.PostAsync("/api/users/logout", null, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);
    }

    [Fact]
    public async Task Logout_DeveRevogarSessao()
    {
        var client = fixture.Factory.CreateClient();
        var credentials = new { Email = "logout_sessao@wilasj.dev", Password = "Big_password!!@21" };

        await client.PostAsJsonAsync("/api/users/register", credentials, TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync("/api/users/login", credentials, TestContext.Current.CancellationToken);

        await client.PostAsync("/api/users/logout", null, TestContext.Current.CancellationToken);

        var result = await client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
