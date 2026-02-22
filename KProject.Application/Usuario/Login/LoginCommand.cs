using KProject.Application.Interfaces;

namespace KProject.Application.Usuario.Login;

public class LoginCommand : ICommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}