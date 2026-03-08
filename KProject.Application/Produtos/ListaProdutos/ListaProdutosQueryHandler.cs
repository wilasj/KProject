using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Produtos;
using KProject.Application.Shared;
using KProject.Common;

namespace KProject.Application.Produtos.ListaProdutos;

public class ListaProdutosQueryHandler(IProdutoRepository produtos) : IQueryHandler<ListaProdutosQuery, Page<ProdutoResponse>>
{
    public async Task<Result<Page<ProdutoResponse>>> Handle(ListaProdutosQuery query, CancellationToken token)
    {
        var page = await produtos.GetPagedAsync(query.Busca, query.Page, query.PageSize, token);
        return Result.Success(page);
    }
}
