using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Clientes;
using KProject.Application.Shared;
using KProject.Common;

namespace KProject.Application.Clientes.ListaClientes;

public class ListaClientesQueryHandler(IClienteRepository clientes) : IQueryHandler<ListaClientesQuery, Page<ClienteResponse>>
{
    public async Task<Result<Page<ClienteResponse>>> Handle(ListaClientesQuery query, CancellationToken token)
    {
        var page = await clientes.GetPagedAsync(query.Busca, query.Page, query.PageSize, token);
        return Result.Success(page);
    }
}
