using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Vendas;
using KProject.Common;

namespace KProject.Application.Vendas.ObtemVenda;

public class ObtemVendaQueryHandler(IVendaRepository vendas)
    : IQueryHandler<ObtemVendaQuery, VendaDetalheResponse>
{
    public async Task<Result<VendaDetalheResponse>> Handle(ObtemVendaQuery query, CancellationToken token)
    {
        var venda = await vendas.GetDetalheAsync(query.VendaId, token);

        if (venda is null)
            return Result.Failure<VendaDetalheResponse>(
                Error.NotFound("Venda.NaoEncontrada", $"Venda com ID {query.VendaId} não encontrada"));

        return Result.Success(venda);
    }
}
