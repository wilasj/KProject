using KProject.Api.Extensions;
using KProject.Application.Clientes.ListaClientes;
using KProject.Application.Interfaces;
using KProject.Application.Shared;

namespace KProject.Api.Endpoints.Clientes;

public class ListaClientes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes", async (
            [AsParameters] ListaClientesRequest request,
            IQueryHandler<ListaClientesQuery, Page<ClienteResponse>> handler,
            CancellationToken token) =>
        {
            var query = new ListaClientesQuery
            {
                Busca = request.Busca,
                Page = request.Pagina ?? 1,
                PageSize = request.Tamanho ?? 10
            };

            var result = await handler.Handle(query, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(result.Value);
        }).RequireAuthorization();
    }
}
