using KProject.Application.Interfaces;

namespace KProject.Application.Usuarios.Register;

public class RegisterCommand: ICommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConviteToken { get; init; }
}