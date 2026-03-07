using System.Reflection;
using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application;
using KProject.Infrastructure;
using KProject.Infrastructure.Shared;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseStaticFiles();

app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} - {StatusCode} - ({Elapsed:0.000}ms)";
    opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("UserId", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
    };
});

app.UseAuthentication();

app.UseAuthorization();

app.MapFallbackToFile("index.html");

var api = app.MapGroup("/api");

app.MapEndpoints(api);

//exception handler

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

namespace KProject.Api
{
    public partial class Program {}
}