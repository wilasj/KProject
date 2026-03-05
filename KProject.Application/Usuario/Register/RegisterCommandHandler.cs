using System.Collections.Immutable;
using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Usuario.Register;

public class RegisterCommandHandler(
    UserManager<IdentityUser<int>> userManager,
    AppDbContext db) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(RegisterCommand command, CancellationToken token)
    {
        var invite = await db.Invites
            .FirstOrDefaultAsync(i => i.Token == command.InviteToken, token);

        if (invite is null || !invite.Disponivel)
            return Result.Failure(new Error(
                "Register.TokenInvalido",
                "O token de convite é inválido ou já foi utilizado.",
                ErrorType.Validation));

        var user = new IdentityUser<int>
        {
            UserName = command.Email,
            Email = command.Email,
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description, ErrorType.Validation))
                .ToImmutableList();
            return Result.Failure(errors);
        }

        invite.Usar();
        
        await db.SaveChangesAsync(token);

        return Result.Success();
    }
}
