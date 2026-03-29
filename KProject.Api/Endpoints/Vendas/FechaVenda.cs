using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Vendas.FechaVenda;

namespace KProject.Api.Endpoints.Vendas;

public class FechaVenda : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/vendas/{id:int}/close", async (
            int id,
            ClaimsPrincipal user,
            ICommandHandler<FechaVendaCommand> handler,
            CancellationToken token) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new FechaVendaCommand
            {
                VendaId = id,
                FechadoPor = userId
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure
                ? result.ToHttpResult()
                : TypedResults.NoContent();
        }).RequireAuthorization();
    }
}
