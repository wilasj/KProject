using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Vendas;
using KProject.Common;

namespace KProject.Application.Vendas.EditaVenda;

public class EditaVendaCommandHandler(
    IVendaRepository vendas,
    IUnitOfWork unitOfWork) : ICommandHandler<EditaVendaCommand>
{
    public async Task<Result> Handle(EditaVendaCommand command, CancellationToken token)
    {
        var venda = await vendas.GetByIdWithItensAsync(command.VendaId, token);
        if (venda is null)
        {
            return Result.Failure(Error.NotFound("Venda.NaoEncontrada",
                $"Venda com ID {command.VendaId} não encontrada")); 
        }

        var alteracoes = command.Itens
            .Select(i => (i.Id, i.Vendido, i.Devolvido))
            .ToList();

        var result = venda.EditarItens(alteracoes, command.AlteradoPor);
        
        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(token);

        return Result.Success();
    }
}
