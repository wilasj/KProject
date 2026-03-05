using KProject.Application.Interfaces;

namespace KProject.Application.Invite.CriaInvite;

public class CriaInviteCommand : ICommand
{
    public required int UsuarioId { get; init; }
}
