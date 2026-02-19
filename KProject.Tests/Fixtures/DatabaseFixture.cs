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
}