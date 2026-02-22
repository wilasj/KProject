using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Shouldly;

namespace KProject.Tests.Usuario;

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
    public async Task Me_DeveRetornar200_ComAutenticacao()
    {
        var client = fixture.Factory.CreateClient();
        var request = new { Email = "me_autenticado@wilasj.dev", Password = "Big_password!!@21" };

        await client.PostAsJsonAsync("/api/users/register", request, TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync("/api/users/login", request, TestContext.Current.CancellationToken);

        var result = await client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);
    }
}
