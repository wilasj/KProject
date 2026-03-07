using System.Net;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Usuarios;

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
        var client = await fixture.CriaClienteAutenticado("logout_valido@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsync("/api/users/logout", null, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);
    }

    [Fact]
    public async Task Logout_DeveRevogarSessao()
    {
        var client = await fixture.CriaClienteAutenticado("logout_sessao@wilasj.dev", "Big_password!!@21");

        await client.PostAsync("/api/users/logout", null, TestContext.Current.CancellationToken);

        var result = await client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
