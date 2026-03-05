using System.Net.Http.Json;
using KProject.Infrastructure.Shared;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace KProject.Tests.Fixtures;

public class DatabaseFixture: IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17").Build();
    public ApiFactory Factory { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Factory = new ApiFactory(_container.GetConnectionString());
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
        await _container.DisposeAsync();
    }

    public async Task ExecuteDbContext(Func<AppDbContext, Task> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(db);
    }

    public async Task<string> CriaInviteToken()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var invite = Domain.Invite.Invite.Criar(0);
        db.Invites.Add(invite);
        await db.SaveChangesAsync();
        return invite.Token;
    }

    public async Task<HttpClient> CriaClienteAutenticado(string email, string password)
    {
        var client = Factory.CreateClient();
        var token = await CriaInviteToken();
        await client.PostAsJsonAsync("/api/users/register", new { Email = email, Password = password, InviteToken = token });
        await client.PostAsJsonAsync("/api/users/login", new { Email = email, Password = password });
        return client;
    }
}