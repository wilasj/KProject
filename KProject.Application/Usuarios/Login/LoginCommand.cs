using KProject.Application.Interfaces;

namespace KProject.Application.Usuarios.Login;

public class LoginCommand : ICommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}