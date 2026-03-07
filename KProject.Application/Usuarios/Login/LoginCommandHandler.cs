using KProject.Application.Interfaces;
using KProject.Common;
using Microsoft.AspNetCore.Identity;

namespace KProject.Application.Usuarios.Login;

public class LoginCommandHandler(SignInManager<IdentityUser<int>> signInManager): ICommandHandler<LoginCommand>
{
    public async Task<Result> Handle(LoginCommand command, CancellationToken token)
    {
        var result = await signInManager.PasswordSignInAsync(command.Email, command.Password, false, false);

        return result.Succeeded ? Result.Success() : Result.Failure(Error.Unauthorized("Usuario.LoginFalhou", "Email ou senha inválidos."));
    }
}