using System.Collections.Immutable;
using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Usuarios.Register;

public class RegisterCommandHandler(
    UserManager<IdentityUser<int>> userManager,
    AppDbContext db) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(RegisterCommand command, CancellationToken token)
    {
        var convite = await db.Convites
            .FirstOrDefaultAsync(i => i.Token == command.ConviteToken, token);

        if (convite is null || !convite.Disponivel)
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

        convite.Usar();
        
        await db.SaveChangesAsync(token);

        return Result.Success();
    }
}
