using KProject.Api.Extensions;
using KProject.Application.Clientes.CriaCliente;
using KProject.Application.Interfaces;

namespace KProject.Api.Endpoints.Clientes;

public class CriaCliente : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clientes", async (
            CriaClienteRequest request,
            ICommandHandler<CriaClienteCommand, int> handler,
            CancellationToken token) =>
        {
            var command = new CriaClienteCommand { Nome = request.Nome };

            var result = await handler.Handle(command, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Created($"/api/clientes/{result.Value}", new { id = result.Value });
        }).RequireAuthorization();
    }
}
