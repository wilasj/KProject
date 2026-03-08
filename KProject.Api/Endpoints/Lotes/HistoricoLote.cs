using KProject.Application.Interfaces;
using KProject.Application.Lotes.HistoricoLote;
using KProject.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace KProject.Api.Endpoints.Lotes;

public class HistoricoLote : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/lotes/{id:int}/historico", async (
            int id,
            [AsParameters] HistoricoLoteRequest request,
            IQueryHandler<HistoricoLoteQuery, HistoricoPage> handler,
            CancellationToken token) =>
        {
            var query = new HistoricoLoteQuery(id, request.Pagina ?? 1, request.TamanhoPagina ?? 20);
            var result = await handler.Handle(query, token);
            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(result.Value);
        }).RequireAuthorization();
    }
}
