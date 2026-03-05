using System.Net;
using System.Net.Http.Json;
using KProject.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace KProject.Tests.Integration.Invite;

[Collection(nameof(DatabaseCollection))]
public class CriaInviteEndpointTests(DatabaseFixture fixture)
{
    private record InviteResponse(string Token);

    [Fact]
    public async Task CriarInvite_DeveRetornar401_SeNaoAutenticado()
    {
        var client = fixture.Factory.CreateClient();
        var result = await client.PostAsJsonAsync("/api/invites", new { }, TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarInvite_DeveRetornar200_SeAutenticado()
    {
        var client = await fixture.CriaClienteAutenticado("invite_auth@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/invites", new { }, TestContext.Current.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        result.StatusCode.ShouldBe(HttpStatusCode.OK, body);

        var response = await result.Content.ReadFromJsonAsync<InviteResponse>(TestContext.Current.CancellationToken);
        response!.Token.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CriarInvite_DeveRetornarMesmoToken_SeJaExisteInviteAtivo()
    {
        var client = await fixture.CriaClienteAutenticado("invite_reutiliza@wilasj.dev", "Big_password!!@21");

        var result1 = await client.PostAsJsonAsync("/api/invites", new { }, TestContext.Current.CancellationToken);
        var result2 = await client.PostAsJsonAsync("/api/invites", new { }, TestContext.Current.CancellationToken);

        var token1 = (await result1.Content.ReadFromJsonAsync<InviteResponse>(TestContext.Current.CancellationToken))!.Token;
        var token2 = (await result2.Content.ReadFromJsonAsync<InviteResponse>(TestContext.Current.CancellationToken))!.Token;

        token2.ShouldBe(token1);
    }

    [Fact]
    public async Task CriarInvite_DevePersistirTokenNoBanco()
    {
        var client = await fixture.CriaClienteAutenticado("invite_persist@wilasj.dev", "Big_password!!@21");

        var result = await client.PostAsJsonAsync("/api/invites", new { }, TestContext.Current.CancellationToken);
        var response = await result.Content.ReadFromJsonAsync<InviteResponse>(TestContext.Current.CancellationToken);

        await fixture.ExecuteDbContext(async db =>
        {
            var invite = await db.Invites.FirstOrDefaultAsync(i => i.Token == response!.Token);
            invite.ShouldNotBeNull();
            invite.UsadoEm.ShouldBeNull();
        });
    }
}
