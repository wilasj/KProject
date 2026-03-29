using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Estoques;
using KProject.Application.Interfaces.Vendas;
using KProject.Common;
using KProject.Domain.Estoques;

namespace KProject.Application.Vendas.CancelaVenda;

public class CancelaVendaCommandHandler(
    IVendaRepository vendasRepository,
    IEstoqueRepository estoquesRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CancelaVendaCommand>
{
    public async Task<Result> Handle(CancelaVendaCommand command, CancellationToken token)
    {
        var venda = await vendasRepository.GetByIdWithItensAsync(command.VendaId, token);
        if (venda is null)
        {
            return Result.Failure(Error.NotFound("Venda.NaoEncontrada",
                $"Venda com ID {command.VendaId} não encontrada"));
        }

        var result = venda.CancelarVenda(command.CanceladoPor);
        if (result.IsFailure)
        {
            return result;
        }

        var devolucaoPorLote = venda.Itens
            .GroupBy(i => i.LoteId)
            .ToDictionary(g => g.Key, g => g.Aggregate(0u, (acc, i) => acc + i.QuantidadeConsignada));

        var estoquesPorLote = await estoquesRepository.GetByLoteIdsAsync(devolucaoPorLote.Keys, token);

        foreach (var (loteId, qtd) in devolucaoPorLote)
        {
            var movResult = estoquesPorLote[loteId].AplicarMovimento(qtd, TipoHistorico.RetornoConsignacao, venda);
            if (movResult.IsFailure)
            {
                return movResult;
            }
        }

        await unitOfWork.SaveChangesAsync(token);

        return Result.Success();
    }
}
