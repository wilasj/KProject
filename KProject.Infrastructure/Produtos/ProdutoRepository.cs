using KProject.Application.Interfaces.Produtos;
using KProject.Application.Produtos.ListaProdutos;
using KProject.Application.Shared;
using KProject.Domain.Produtos;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Produtos;

public class ProdutoRepository(AppDbContext db) : IProdutoRepository
{
    public async Task AddAsync(Produto produto, CancellationToken token = default) =>
        await db.Produtos.AddAsync(produto, token);

    public Task<bool> ExistsAsync(int id, CancellationToken token = default) =>
        db.Produtos.AnyAsync(p => p.Id == id, token);

    public async Task<Page<ProdutoResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default)
    {
        var spec = new ProdutoSpecification(busca);
        var q = db.Produtos.AsQueryable();

        if (spec.Criteria != null)
            q = q.Where(spec.Criteria);

        if (spec.OrderBy != null)
            q = spec.Ascending
                ? q.OrderBy(spec.OrderBy)
                : q.OrderByDescending(spec.OrderBy);

        var total = await q.CountAsync(token);

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProdutoResponse(p.Id, p.Nome, p.Referencia, p.Descricao, p.CodigoAnvisa, p.CriadoEm))
            .ToListAsync(token);

        return new Page<ProdutoResponse>(items, total);
    }
}
