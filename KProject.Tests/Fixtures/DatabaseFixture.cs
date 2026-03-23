using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using KProject.Domain.Convites;
using KProject.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace KProject.Tests.Fixtures;

public class DatabaseFixture: IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17").Build();
    public ApiFactory Factory { get; private set; } = null!;

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

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

    public async Task<int> CriaUsuarioFixture(string email = "fixture@wilasj.dev", string password = "Big_password!!@21")
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<int>>>();

        var existente = await userManager.FindByEmailAsync(email);
        if (existente is not null)
            return existente.Id;

        var user = new IdentityUser<int> { UserName = email, Email = email };
        await userManager.CreateAsync(user, password);
        return user.Id;
    }

    public async Task<string> CriaConviteToken()
    {
        var userId = await CriaUsuarioFixture();

        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var convite = Convite.Criar(userId);
        db.Convites.Add(convite);
        await db.SaveChangesAsync();
        return convite.Token;
    }

    public async Task<HttpClient> CriaClienteAutenticado(string email, string password)
    {
        var client = Factory.CreateClient();
        var token = await CriaConviteToken();
        await client.PostAsJsonAsync("/api/users/register", new { Email = email, Password = password, ConviteToken = token });
        await client.PostAsJsonAsync("/api/users/login", new { Email = email, Password = password });
        return client;
    }
}