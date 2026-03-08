using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Lotes.CriaLote;

namespace KProject.Api.Endpoints.Lotes;

public class CriaLote : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/lotes", async (
            CriaLoteRequest request,
            ICommandHandler<CriaLoteCommand, int> handler,
            CancellationToken token) =>
        {
            var command = new CriaLoteCommand
            {
                ProdutoId = request.ProdutoId,
                Numero = request.Numero,
                Validade = request.Validade,
                QuantidadeInicial = request.QuantidadeInicial
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure
                ? result.ToHttpResult()
                : TypedResults.Created($"/api/lotes/{result.Value}");
        }).RequireAuthorization();
    }
}
