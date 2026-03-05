using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Integration.Usuario;

[Collection(nameof(DatabaseCollection))]
public class MeEndpointTests(DatabaseFixture fixture)
{
    private record ErrorResponse(string Code, string Description);

    [Fact]
    public async Task Me_DeveRetornar401_SemAutenticacao()
    {
        var client = fixture.Factory.CreateClient();

        var result = await client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_DeveRetornar200_ComEmailDoUsuario()
    {
        var client = await fixture.CriaClienteAutenticado("me_autenticado@wilasj.dev", "Big_password!!@21");

        var result = await client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var response = await result.Content.ReadFromJsonAsync<MeResponse>(TestContext.Current.CancellationToken);
        response!.Email.ShouldBe("me_autenticado@wilasj.dev");
    }

    private record MeResponse(string Email);
}
