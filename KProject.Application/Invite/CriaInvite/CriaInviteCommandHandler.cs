using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Invite.CriaInvite;

public class CriaInviteCommandHandler(AppDbContext db) : ICommandHandler<CriaInviteCommand, string>
{
    public async Task<Result<string>> Handle(CriaInviteCommand command, CancellationToken token)
    {
        var existente = await db.Invites
            .FirstOrDefaultAsync(i => i.CriadoPorId == command.UsuarioId && i.UsadoEm == null, token);

        if (existente is not null)
            return existente.Token;

        var invite = Domain.Invite.Invite.Criar(command.UsuarioId);
        db.Invites.Add(invite);
        await db.SaveChangesAsync(token);
        return invite.Token;
    }
}
