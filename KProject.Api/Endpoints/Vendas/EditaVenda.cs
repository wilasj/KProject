using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Vendas.EditaVenda;

namespace KProject.Api.Endpoints.Vendas;

public class EditaVenda : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/vendas/{id:int}", async (
            int id,
            EditaVendaRequest request,
            ClaimsPrincipal user,
            ICommandHandler<EditaVendaCommand> handler,
            CancellationToken token) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new EditaVendaCommand
            {
                VendaId = id,
                AlteradoPor = userId,
                Itens = request.Itens
                    .Select(i => new EditaItemDto(i.Id, i.Vendido, i.Devolvido))
                    .ToList()
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure
                ? result.ToHttpResult()
                : TypedResults.NoContent();
        }).RequireAuthorization();
    }
}
