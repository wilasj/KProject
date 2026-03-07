using KProject.Application.Interfaces;
using KProject.Application.Shared;
using KProject.Common;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Produtos.ListaProdutos;

public class ListaProdutosQueryHandler(AppDbContext context) : IQueryHandler<ListaProdutosQuery, Page<ProdutoResponse>>
{
    public async Task<Result<Page<ProdutoResponse>>> Handle(ListaProdutosQuery query, CancellationToken token)
    {
        var spec = new ProdutoSpecification(query.Busca);

        var q = context.Produtos.AsQueryable();

        if (spec.Criteria != null)
            q = q.Where(spec.Criteria);

        if(spec.OrderBy != null)
            q = spec.Ascending
                ? q.OrderBy(spec.OrderBy)
                : q.OrderByDescending(spec.OrderBy);

        var total = await q.CountAsync(token);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProdutoResponse(p.Id, p.Nome, p.Referencia, p.Descricao, p.CodigoAnvisa, p.CriadoEm))
            .ToListAsync(token);

        return Result.Success(new Page<ProdutoResponse>(items, total));
    }
}
