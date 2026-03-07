using KProject.Application.Interfaces;

namespace KProject.Application.Lotes.ListaLotes;

public record ListaLotesQuery(int ProdutoId) : IQuery<IReadOnlyList<LoteResponse>>;
