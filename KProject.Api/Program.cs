using System.Reflection;
using KProject.Api.Extensions;
using KProject.Infrastructure;
using KProject.Infrastructure.Database;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

var api = app.MapGroup("/api");

app.MapEndpoints(api);

app.MapGet("/", () => "Hello World!");

//logs
//exception handler

app.UseAuthentication();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    if ((await context.Database.GetPendingMigrationsAsync()).Any())
    {
        await context.Database.MigrateAsync();
    }
}

await app.RunAsync();