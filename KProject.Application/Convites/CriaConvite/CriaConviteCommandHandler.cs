using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Convites;
using KProject.Common;
using KProject.Domain.Convites;

namespace KProject.Application.Convites.CriaConvite;

public class CriaConviteCommandHandler(
    IConviteRepository convites,
    IUnitOfWork unitOfWork) : ICommandHandler<CriaConviteCommand, string>
{
    public async Task<Result<string>> Handle(CriaConviteCommand command, CancellationToken token)
    {
        var existente = await convites.FindByUsuarioIdAsync(command.UsuarioId, token);

        if (existente is not null)
            return existente.Token;

        var convite = Convite.Criar(command.UsuarioId);
        await convites.AddAsync(convite, token);
        await unitOfWork.SaveChangesAsync(token);

        return convite.Token;
    }
}
