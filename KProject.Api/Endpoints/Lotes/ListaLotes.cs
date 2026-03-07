using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Lotes.ListaLotes;

namespace KProject.Api.Endpoints.Lotes;

public class ListaLotes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/produtos/{id:int}/lotes", async (
            int id,
            IQueryHandler<ListaLotesQuery, IReadOnlyList<LoteResponse>> handler,
            CancellationToken token) =>
        {
            var query = new ListaLotesQuery(id);
            
            var result = await handler.Handle(query, token);
            
            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(result.Value);
        }).RequireAuthorization();
    }
}
