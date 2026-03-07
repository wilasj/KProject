using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Domain.Convites;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Convites.CriaConvite;

public class CriaConviteCommandHandler(AppDbContext db) : ICommandHandler<CriaConviteCommand, string>
{
    public async Task<Result<string>> Handle(CriaConviteCommand command, CancellationToken token)
    {
        var existente = await db.Convites
            .FirstOrDefaultAsync(i => i.CriadoPorId == command.UsuarioId && i.UsadoEm == null, token);

        if (existente is not null)
            return existente.Token;

        var convite = Convite.Criar(command.UsuarioId);
        db.Convites.Add(convite);
        await db.SaveChangesAsync(token);
        return convite.Token;
    }
}
