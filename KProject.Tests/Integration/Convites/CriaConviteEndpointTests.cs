using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Convites;

[Collection(nameof(DatabaseCollection))]
public class CriaConviteEndpointTests(DatabaseFixture fixture)
{
    private record ConviteResponse(string Token);

    [Fact]
    public async Task CriarConvite_DeveRetornar401_SeNaoAutenticado()
    {
        var client = fixture.Factory.CreateClient();
        var result = await client.PostAsJsonAsync("/api/convites", new { }, TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarConvite_DeveRetornar200_SeAutenticado()
    {
        var client = await fixture.CriaClienteAutenticado("convite_auth@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/convites", new { }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var response = await result.Content.ReadFromJsonAsync<ConviteResponse>(TestContext.Current.CancellationToken);
        response!.Token.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CriarConvite_DeveRetornarMesmoToken_SeJaExisteConviteAtivo()
    {
        var client = await fixture.CriaClienteAutenticado("convite_reutiliza@wilasj.dev", "Big_password!!@21");

        var result1 = await client.PostAsJsonAsync("/api/convites", new { }, TestContext.Current.CancellationToken);
        var result2 = await client.PostAsJsonAsync("/api/convites", new { }, TestContext.Current.CancellationToken);

        var token1 = (await result1.Content.ReadFromJsonAsync<ConviteResponse>(TestContext.Current.CancellationToken))!.Token;
        var token2 = (await result2.Content.ReadFromJsonAsync<ConviteResponse>(TestContext.Current.CancellationToken))!.Token;

        token2.ShouldBe(token1);
    }

    [Fact]
    public async Task CriarConvite_DevePersistirTokenNoBanco()
    {
        var client = await fixture.CriaClienteAutenticado("convite_persist@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/convites", new { }, TestContext.Current.CancellationToken);
        var response = await result.Content.ReadFromJsonAsync<ConviteResponse>(TestContext.Current.CancellationToken);

        await fixture.ExecuteDbContext(async db =>
        {
            var convite = await db.Convites.FirstOrDefaultAsync(i => i.Token == response!.Token);
            convite.ShouldNotBeNull();
            convite.UsadoEm.ShouldBeNull();
        });
    }
}
