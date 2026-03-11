using KProject.Application.Interfaces;

namespace KProject.Application.Clientes.CriaCliente;

public class CriaClienteCommand : ICommand
{
    public required string Nome { get; init; }
}
