using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Vendas;
using KProject.Application.Shared;
using KProject.Common;

namespace KProject.Application.Vendas.ListaVendas;

public class ListaVendasQueryHandler(IVendaRepository vendas) : IQueryHandler<ListaVendasQuery, Page<VendaResponse>>
{
    public async Task<Result<Page<VendaResponse>>> Handle(ListaVendasQuery query, CancellationToken token)
    {
        var page = await vendas.GetPagedAsync(query.Busca, query.Page, query.PageSize, token);
        return Result.Success(page);
    }
}
