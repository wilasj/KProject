using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Produtos.ListaProdutos;
using KProject.Application.Shared;

namespace KProject.Api.Endpoints.Produtos;

public class ListaProdutos : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/produtos", async (
            [AsParameters] ListaProdutosRequest request,
            IQueryHandler<ListaProdutosQuery, Page<ProdutoResponse>> handler,
            CancellationToken token) =>
        {
            var query = new ListaProdutosQuery
            {
                Busca = request.Busca,
                Page = request.Pagina ?? 1,
                PageSize = request.Tamanho ?? 10
            };

            var result = await handler.Handle(query, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(result.Value);
        }).RequireAuthorization();
    }
}
