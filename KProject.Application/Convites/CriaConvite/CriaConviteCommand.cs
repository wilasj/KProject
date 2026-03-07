using KProject.Application.Interfaces;

namespace KProject.Application.Convites.CriaConvite;

public class CriaConviteCommand : ICommand
{
    public required int UsuarioId { get; init; }
}
