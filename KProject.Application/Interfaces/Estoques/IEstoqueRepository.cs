using KProject.Domain.Estoques;

namespace KProject.Application.Interfaces.Estoques;

public interface IEstoqueRepository
{
    Task<Dictionary<int, Estoque>> GetByLoteIdsAsync(
        IEnumerable<int> loteIds, CancellationToken token = default);
}
