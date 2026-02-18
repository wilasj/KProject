using System.Collections.Immutable;
using KProject.Application.Interfaces;
using KProject.Common;
using Microsoft.AspNetCore.Identity;

namespace KProject.Application.User.Register;

public class RegisterCommandHandler(UserManager<IdentityUser<int>> userManager): ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(RegisterCommand command, CancellationToken token)
    {
        var user = new IdentityUser<int>
        {
            UserName = command.Email,
            Email = command.Email,
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description, ErrorType.Validation)).ToImmutableList();
            
            return Result.Failure(errors);
        }

        return Result.Success();
    }
}