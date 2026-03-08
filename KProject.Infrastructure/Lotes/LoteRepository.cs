using KProject.Application.Interfaces.Lotes;
using KProject.Application.Lotes.ListaLotes;
using KProject.Domain.Lotes;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Lotes;

public class LoteRepository(AppDbContext db) : ILoteRepository
{
    public async Task AddAsync(Lote lote, CancellationToken token = default) =>
        await db.Lotes.AddAsync(lote, token);

    public async Task<IReadOnlyList<LoteResponse>> GetByProdutoIdAsync(int produtoId, CancellationToken token = default) =>
        await db.Lotes
            .Where(l => l.ProdutoId == produtoId)
            .OrderBy(l => l.Validade)
            .Select(l => new LoteResponse(l.Id, l.Numero, l.Validade, l.Estoque.QuantidadeAtual))
            .ToListAsync(token);
}
