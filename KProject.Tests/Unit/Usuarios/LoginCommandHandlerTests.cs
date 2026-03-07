using KProject.Application.Usuarios.Login;
using KProject.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Usuarios;

public class LoginCommandHandlerTests
{
    private readonly SignInManager<IdentityUser<int>> _signInManager;

    public LoginCommandHandlerTests()
    {
        var store = Substitute.For<IUserStore<IdentityUser<int>>>();
        var userManager = Substitute.For<UserManager<IdentityUser<int>>>(store, null, null, null, null, null, null, null, null);
        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<IdentityUser<int>>>();
        var options = Substitute.For<IOptions<IdentityOptions>>();
        var logger = Substitute.For<ILogger<SignInManager<IdentityUser<int>>>>();
        var schemes = Substitute.For<IAuthenticationSchemeProvider>();
        var confirmation = Substitute.For<IUserConfirmation<IdentityUser<int>>>();

        options.Value.Returns(new IdentityOptions());

        _signInManager = Substitute.For<SignInManager<IdentityUser<int>>>(
            userManager, contextAccessor, claimsFactory, options, logger, schemes, confirmation);
    }

    [Fact]
    public async Task Login_DeveRetornarSucesso_SeCredenciaisValidas()
    {
        _signInManager
            .PasswordSignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(SignInResult.Success);

        var command = new LoginCommand { Email = "will@wilasj.dev", Password = "Big_password!!@21" };
        var handler = new LoginCommandHandler(_signInManager);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task Login_DeveRetornarFalha_SeCredenciaisInvalidas()
    {
        _signInManager
            .PasswordSignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(SignInResult.Failed);

        var command = new LoginCommand { Email = "will@wilasj.dev", Password = "senha_errada" };
        var handler = new LoginCommandHandler(_signInManager);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.ShouldContain(new Error("Usuario.LoginFalhou", "Email ou senha inválidos.", ErrorType.Unauthorized));
    }
}
