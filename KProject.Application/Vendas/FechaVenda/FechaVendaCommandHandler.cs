using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Estoques;
using KProject.Application.Interfaces.Vendas;
using KProject.Common;
using KProject.Domain.Estoques;

namespace KProject.Application.Vendas.FechaVenda;

public class FechaVendaCommandHandler(
    IVendaRepository vendasRepository,
    IEstoqueRepository estoquesRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<FechaVendaCommand>
{
    public async Task<Result> Handle(FechaVendaCommand command, CancellationToken token)
    {
        var venda = await vendasRepository.GetByIdWithItensAsync(command.VendaId, token);
        if (venda is null)
        {
            return Result.Failure(Error.NotFound("Venda.NaoEncontrada",
                $"Venda com ID {command.VendaId} não encontrada"));
        }

        var retornoPorLote = venda.Itens
            .Where(i => i.EmAberto > 0 || i.Devolvido > 0)
            .GroupBy(i => i.LoteId)
            .ToDictionary(g => g.Key, g => g.Aggregate(0u, (acc, i) => acc + i.EmAberto + i.Devolvido));

        var loteIds = retornoPorLote.Keys.ToList();

        var estoquesPorLote = loteIds.Count > 0
            ? await estoquesRepository.GetByLoteIdsAsync(loteIds, token)
            : [];

        var result = venda.FecharVenda(command.FechadoPor);

        if (result.IsFailure)
        {
            return result;
        }

        foreach (var (loteId, qtd) in retornoPorLote)
        {
            var movResult = estoquesPorLote[loteId].AplicarMovimento(qtd, TipoHistorico.RetornoConsignacao, venda, command.FechadoPor);
            if (movResult.IsFailure)
            {
                return movResult;
            }
        }

        await unitOfWork.SaveChangesAsync(token);

        return Result.Success();
    }
}
