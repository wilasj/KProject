using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Produtos.CriaProduto;

namespace KProject.Api.Endpoints.Produtos;

public class CriaProduto : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/produtos", async (
            CriaProdutoRequest request,
            ICommandHandler<CriaProdutoCommand, int> handler,
            CancellationToken token) =>
        {
            var command = new CriaProdutoCommand
            {
                Nome = request.Nome,
                Referencia = request.Referencia,
                Descricao = request.Descricao,
                CodigoAnvisa = request.CodigoAnvisa
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Created($"/api/produtos/{result.Value}", new { id = result.Value });
        }).RequireAuthorization();
    }
}
