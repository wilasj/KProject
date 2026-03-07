using KProject.Application.Interfaces;

namespace KProject.Application.Convite.CriaConvite;

public class CriaConviteCommand : ICommand
{
    public required int UsuarioId { get; init; }
}
