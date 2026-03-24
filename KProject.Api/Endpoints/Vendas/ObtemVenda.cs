using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Vendas.ObtemVenda;

namespace KProject.Api.Endpoints.Vendas;

public class ObtemVenda : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/vendas/{id:int}", async (
            int id,
            IQueryHandler<ObtemVendaQuery, VendaDetalheResponse> handler,
            CancellationToken token) =>
        {
            var result = await handler.Handle(new ObtemVendaQuery(id), token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(result.Value);
        }).RequireAuthorization();
    }
}
