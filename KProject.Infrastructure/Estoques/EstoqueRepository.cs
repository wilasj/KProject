using KProject.Application.Interfaces.Estoques;
using KProject.Domain.Estoques;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Estoques;

public class EstoqueRepository(AppDbContext db) : IEstoqueRepository
{
    public async Task<Dictionary<int, Estoque>> GetByLoteIdsAsync(
        IEnumerable<int> loteIds, CancellationToken token = default) =>
        await db.Estoques
            .Where(e => loteIds.Contains(e.LoteId))
            .ToDictionaryAsync(e => e.LoteId, token);
}
