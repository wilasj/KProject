using KProject.Application.Usuarios.Register;
using KProject.Infrastructure.Shared;
using KProject.Tests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace KProject.Tests.Integration.Usuarios;

[Collection(nameof(DatabaseCollection))]
public class RegisterCommandHandlerTests(DatabaseFixture fixture)
{
    private RegisterCommandHandler CriaHandler(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser<int>>>();
        var db = services.GetRequiredService<AppDbContext>();
        return new RegisterCommandHandler(userManager, db);
    }

    [Fact]
    public async Task Registrar_DeveRetornarSucesso_SeTokenValido()
    {
        var token = await fixture.CriaConviteToken();
        var command = new RegisterCommand
        {
            Email = "handler_sucesso@wilasj.dev",
            Password = "Big_password!!@21",
            ConviteToken = token,
        };

        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var result = await CriaHandler(scope.ServiceProvider).Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task Registrar_DeveRetornarFalha_SeTokenInvalido()
    {
        var command = new RegisterCommand
        {
            Email = "handler_token_invalido@wilasj.dev",
            Password = "Big_password!!@21",
            ConviteToken = "token-inexistente",
        };

        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var result = await CriaHandler(scope.ServiceProvider).Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "Register.TokenInvalido");
    }

    [Fact]
    public async Task Registrar_DeveRetornarFalha_SeTokenJaUsado()
    {
        var token = await fixture.CriaConviteToken();

        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var handler = CriaHandler(scope.ServiceProvider);

        await handler.Handle(new RegisterCommand
        {
            Email = "handler_token_uso1@wilasj.dev",
            Password = "Big_password!!@21",
            ConviteToken = token,
        }, TestContext.Current.CancellationToken);

        var result = await handler.Handle(new RegisterCommand
        {
            Email = "handler_token_uso2@wilasj.dev",
            Password = "Big_password!!@21",
            ConviteToken = token,
        }, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "Register.TokenInvalido");
    }
}
