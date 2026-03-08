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
        var items = await db.Lotes
            .Where(l => l.Id == loteId)
            .SelectMany(l => l.Estoque.Historico)
            .OrderByDescending(h => h.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina + 1)
            .Select(h => new HistoricoEstoqueResponse(h.Id, h.Tipo.ToString(), h.DeltaQuantidade, h.CriadoEm))
            .ToListAsync(token);

        var hasMore = items.Count > tamanhoPagina;
        if (hasMore) items.RemoveAt(items.Count - 1);

        return new HistoricoPage(items, hasMore);
    }
}
