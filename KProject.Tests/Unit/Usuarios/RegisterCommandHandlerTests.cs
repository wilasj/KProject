using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Convites;
using KProject.Application.Usuarios.Register;
using KProject.Common;
using KProject.Domain.Convites;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Usuarios;

public class RegisterCommandHandlerTests
{
    private readonly UserManager<IdentityUser<int>> _userManager;
    private readonly IConviteRepository _convites = Substitute.For<IConviteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public RegisterCommandHandlerTests()
    {
        var store = Substitute.For<IUserStore<IdentityUser<int>>>();
        _userManager = Substitute.For<UserManager<IdentityUser<int>>>(store, null, null, null, null, null, null, null, null);
    }

    private Task<Result> Handle(RegisterCommand command) =>
        new RegisterCommandHandler(_userManager, _convites, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    private static RegisterCommand ComandoPadrao(string conviteToken) => new()
    {
        Email = "user@example.com",
        Password = "Big_password!!@21",
        ConviteToken = conviteToken,
    };

    [Fact]
    public async Task Register_ConviteNaoEncontrado_RetornaTokenInvalido()
    {
        _convites.FindByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Convite?)null);

        var result = await Handle(ComandoPadrao("token-inexistente"));

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain(e => e.Code == "Register.TokenInvalido");
    }

    [Fact]
    public async Task Register_ConviteJaUsado_RetornaTokenInvalido()
    {
        var convite = Convite.Criar(1);
        convite.Usar();
        _convites.FindByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(convite);

        var result = await Handle(ComandoPadrao(convite.Token));

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain(e => e.Code == "Register.TokenInvalido");
    }

    [Fact]
    public async Task Register_CriacaoDeUsuarioFalha_RetornaErrosDoIdentity()
    {
        var convite = Convite.Criar(1);
        _convites.FindByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(convite);
        _userManager.CreateAsync(Arg.Any<IdentityUser<int>>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError { Code = "SenhaFraca", Description = "Senha muito fraca." }));

        var result = await Handle(ComandoPadrao(convite.Token));

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain(e => e.Code == "SenhaFraca");
    }

    [Fact]
    public async Task Register_ConviteValido_RetornaSucessoEPersiste()
    {
        var convite = Convite.Criar(1);
        _convites.FindByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(convite);
        _userManager.CreateAsync(Arg.Any<IdentityUser<int>>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        var result = await Handle(ComandoPadrao(convite.Token));

        result.IsSuccess.ShouldBeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        convite.Disponivel.ShouldBeFalse();
    }
}
