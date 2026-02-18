using KProject.Application.User.Register;
using KProject.Common;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Usuario;

public class RegisterCommandHandlerTests
{
    private readonly UserManager<IdentityUser<int>> _userManager;
    
    public RegisterCommandHandlerTests()
    {
        var store = Substitute.For<IUserStore<IdentityUser<int>>>();
        _userManager = Substitute.For<UserManager<IdentityUser<int>>>(store, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Registrar_DeveRetornarSucesso_SeCommandValido()
    {
        _userManager
            .CreateAsync(Arg.Any<IdentityUser<int>>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        
        var command = new RegisterCommand
        {
            Email = "will@wilasj.dev",
            Password = "nice_password!!"
        };

        var handler = new RegisterCommandHandler(_userManager);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        
        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task Registrar_DeveRetornarFalha_SeCommandInvalido()
    {
        _userManager
            .CreateAsync(Arg.Any<IdentityUser<int>>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Algo deu errado" }));

        var command = new RegisterCommand
        {
            Email = "will@wilasj.dev",
            Password = "nice_password!!"
        };

        var handler = new RegisterCommandHandler(_userManager);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.ShouldContain(new Error("Error", "Algo deu errado", ErrorType.Validation));
    }
}