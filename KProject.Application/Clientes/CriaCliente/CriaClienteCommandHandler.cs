using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Clientes;
using KProject.Common;
using KProject.Domain.Clientes;

namespace KProject.Application.Clientes.CriaCliente;

public class CriaClienteCommandHandler(IClienteRepository clientes, IUnitOfWork unitOfWork) : ICommandHandler<CriaClienteCommand, int>
{
    public async Task<Result<int>> Handle(CriaClienteCommand command, CancellationToken token)
    {
        var cliente = Cliente.Criar(command.Nome);

        await clientes.AddAsync(cliente, token);
        await unitOfWork.SaveChangesAsync(token);

        return Result.Success(cliente.Id);
    }
}
