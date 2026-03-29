using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Clientes;
using KProject.Application.Interfaces.Estoques;
using KProject.Application.Interfaces.Vendas;
using KProject.Common;
using KProject.Domain.Estoques;
using KProject.Domain.Vendas;

namespace KProject.Application.Vendas.CriaVenda;

public class CriaVendaCommandHandler(
    IClienteRepository clientes,
    IEstoqueRepository estoques,
    IVendaRepository vendas,
    IUnitOfWork unitOfWork) : ICommandHandler<CriaVendaCommand, int>
{
    public async Task<Result<int>> Handle(CriaVendaCommand command, CancellationToken token)
    {
        var clienteExiste = await clientes.ExistsAsync(command.ClienteId, token);
        if (!clienteExiste)
            return Result.Failure<int>(Error.NotFound("Venda.ClienteNaoEncontrado",
                $"Cliente com ID {command.ClienteId} não encontrado"));

        var loteIds = command.Itens.Select(i => i.LoteId).Distinct().ToList();
        var estoquesPorLote = await estoques.GetByLoteIdsAsync(loteIds, token);

        foreach (var loteId in loteIds)
        {
            if (!estoquesPorLote.ContainsKey(loteId))
                return Result.Failure<int>(Error.NotFound("Venda.LoteNaoEncontrado",
                    $"Lote com ID {loteId} não encontrado"));
        }

        var itensDictionary = command.Itens.ToDictionary(
            i => (i.LoteId, i.PacienteNome),
            i => i.Quantidade);

        var vendaResult = Venda.Criar(command.ClienteId, command.CriadaPor, itensDictionary);
        if (vendaResult.IsFailure)
            return Result.Failure<int>(vendaResult.Errors);

        var qtdPorLote = command.Itens
            .GroupBy(i => i.LoteId)
            .ToDictionary(g => g.Key, g => g.Aggregate(0u, (acc, i) => acc + i.Quantidade));

        var venda = vendaResult.Value;

        foreach (var (loteId, qtd) in qtdPorLote)
        {
            var movResult = estoquesPorLote[loteId].AplicarMovimento(qtd, TipoHistorico.SaidaConsignacao, venda, command.CriadaPor);
            if (movResult.IsFailure)
            {
                return Result.Failure<int>(movResult.Errors);
            }
        }

        await vendas.AddAsync(venda, token);
        await unitOfWork.SaveChangesAsync(token);

        return Result.Success(venda.Id);
    }
}
