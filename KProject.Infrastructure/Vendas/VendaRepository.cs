using KProject.Application.Interfaces.Vendas;
using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;
using KProject.Application.Vendas.ObtemVenda;
using KProject.Domain.Vendas;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Vendas;

public class VendaRepository(AppDbContext db) : IVendaRepository
{
    public async Task AddAsync(Venda venda, CancellationToken token = default) =>
        await db.Vendas.AddAsync(venda, token);

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

    public async Task<VendaDetalheResponse?> GetDetalheAsync(int id, CancellationToken token = default)
    {
        var venda = await db.Vendas
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Historico)
            .FirstOrDefaultAsync(v => v.Id == id, token);

        if (venda is null) return null;

        var loteIds = venda.Itens.Select(i => i.LoteId).Distinct().ToList();

        var lotePorId = await (
            from l in db.Lotes
            join p in db.Produtos on l.ProdutoId equals p.Id
            where loteIds.Contains(l.Id)
            select new { l.Id, l.Numero, ProdutoNome = p.Nome }
        ).ToDictionaryAsync(x => x.Id, token);

        var criadoPor = await db.Users
            .Where(u => u.Id == venda.CriadaPor)
            .Select(u => u.UserName!)
            .FirstOrDefaultAsync(token) ?? string.Empty;

        var itens = venda.Itens
            .Select(i =>
            {
                var lote = lotePorId.GetValueOrDefault(i.LoteId);
                return new ItemDetalheResponse(
                    i.Id,
                    lote?.ProdutoNome ?? string.Empty,
                    lote?.Numero ?? 0,
                    i.PacienteNome,
                    i.QuantidadeConsignada,
                    i.Vendido,
                    i.Devolvido,
                    i.EmAberto
                );
            }).ToList();

        return new VendaDetalheResponse(
            venda.Id,
            venda.Status,
            venda.CriadaEm,
            criadoPor,
            venda.Itens.Select(i => i.UltimaAlteracao).Max(),
            venda.Cliente?.Nome ?? string.Empty,
            itens.Aggregate(0u, (acc, i) => acc + i.QuantidadeConsignada),
            itens.Aggregate(0u, (acc, i) => acc + i.Vendido),
            itens.Aggregate(0u, (acc, i) => acc + i.Devolvido),
            itens
        );
    }
}
