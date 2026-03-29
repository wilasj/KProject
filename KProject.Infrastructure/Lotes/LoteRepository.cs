using KProject.Application.Interfaces.Lotes;
using KProject.Application.Lotes.HistoricoLote;
using KProject.Application.Lotes.ListaLotes;
using KProject.Domain.Lotes;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Lotes;

public class LoteRepository(AppDbContext db) : ILoteRepository
{
    public async Task AddAsync(Lote lote, CancellationToken token = default) =>
        await db.Lotes.AddAsync(lote, token);

    public async Task<bool> ExistsAsync(int id, CancellationToken token = default) =>
        await db.Lotes.AnyAsync(l => l.Id == id, token);

    public async Task<IReadOnlyList<LoteResponse>> GetByProdutoIdAsync(int produtoId, CancellationToken token = default) =>
        await db.Lotes
            .Where(l => l.ProdutoId == produtoId)
            .OrderBy(l => l.Validade)
            .Select(l => new LoteResponse(l.Id, l.Numero, l.Validade, l.Estoque.QuantidadeAtual))
            .ToListAsync(token);

    public async Task<HistoricoPage> GetHistoricoPagedAsync(int loteId, int pagina, int tamanhoPagina, CancellationToken token = default)
    {
        var query =
            from h in db.Lotes
                .Where(l => l.Id == loteId)
                .SelectMany(l => l.Estoque.Historico)
            join u in db.Users on h.CriadoPor equals u.Id into users
            from u in users.DefaultIfEmpty()
            orderby h.CriadoEm descending
            select new HistoricoEstoqueResponse(
                h.Id,
                h.Tipo.ToString(),
                h.DeltaQuantidade,
                h.CriadoEm,
                h.Venda != null ? h.Venda.Id : null,
                u != null ? u.UserName : null);

        var items = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina + 1)
            .ToListAsync(token);

        var hasMore = items.Count > tamanhoPagina;
        if (hasMore) items.RemoveAt(items.Count - 1);

        return new HistoricoPage(items, hasMore);
    }
}
