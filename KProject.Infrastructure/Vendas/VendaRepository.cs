using KProject.Application.Interfaces.Vendas;
using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;
using KProject.Domain.Vendas;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Vendas;

public class VendaRepository(AppDbContext db) : IVendaRepository
{
    public async Task<Page<VendaResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default)
    {
        var spec = new VendaSpecification(busca);
        var q = db.Vendas.AsQueryable();

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
            .Select(v => new VendaResponse(v.Id, v.Cliente!.Nome, v.CriadaEm, v.Status, v.Itens.Count))
            .ToListAsync(token);

        return new Page<VendaResponse>(items, total);
    }
}
