using KProject.Application.Interfaces;

namespace KProject.Application.User.Register;

public class RegisterCommand: ICommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}