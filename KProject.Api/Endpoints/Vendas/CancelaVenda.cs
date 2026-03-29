using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Vendas.CancelaVenda;

namespace KProject.Api.Endpoints.Vendas;

public class CancelaVenda : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/vendas/{id:int}/cancel", async (
            int id,
            ClaimsPrincipal user,
            ICommandHandler<CancelaVendaCommand> handler,
            CancellationToken token) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new CancelaVendaCommand
            {
                VendaId = id,
                CanceladoPor = userId
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure
                ? result.ToHttpResult()
                : TypedResults.NoContent();
        }).RequireAuthorization();
    }
}
