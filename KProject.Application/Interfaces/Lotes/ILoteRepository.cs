using KProject.Application.Lotes.ListaLotes;
using KProject.Domain.Lotes;

namespace KProject.Application.Interfaces.Lotes;

public interface ILoteRepository
{
    Task AddAsync(Lote lote, CancellationToken token = default);
    Task<IReadOnlyList<LoteResponse>> GetByProdutoIdAsync(int produtoId, CancellationToken token = default);
}
