using KProject.Api.Endpoints.Produto;
using KProject.Application.Interfaces;
using KProject.Application.Produto.ListaProdutos;
using KProject.Application.Shared;

namespace KProject.Api.Endpoints.Produto;

public class ListaProdutos : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/produtos", async (
            [AsParameters] ListaProdutosRequest request,
            IQueryHandler<ListaProdutosQuery, PagedResult<ProdutoResponse>> handler,
            CancellationToken token) =>
        {
            var query = new ListaProdutosQuery
            {
                Busca = request.Busca,
                Page = request.Pagina ?? 1,
                PageSize = request.Tamanho ?? 10
            };

            var result = await handler.Handle(query, token);

            return TypedResults.Ok(result);
        }).RequireAuthorization();
    }
}
