using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Vendas.CriaVenda;

namespace KProject.Api.Endpoints.Vendas;

public class CriaVenda : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/vendas", async (
            CriaVendaRequest request,
            ClaimsPrincipal user,
            ICommandHandler<CriaVendaCommand, int> handler,
            CancellationToken token) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new CriaVendaCommand
            {
                ClienteId = request.ClienteId,
                CriadaPor = userId,
                Itens = request.Itens
                    .Select(i => new NovoItemDto(i.LoteId, i.PacienteNome, i.Quantidade))
                    .ToList()
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure
                ? result.ToHttpResult()
                : TypedResults.Created($"/api/vendas/{result.Value}", new { id = result.Value });
        }).RequireAuthorization();
    }
}
