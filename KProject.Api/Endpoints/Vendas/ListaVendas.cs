using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;

namespace KProject.Api.Endpoints.Vendas;

public class ListaVendas : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/vendas", async (
            [AsParameters] ListaVendasRequest request,
            IQueryHandler<ListaVendasQuery, Page<VendaResponse>> handler,
            CancellationToken token) =>
        {
            var query = new ListaVendasQuery
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
