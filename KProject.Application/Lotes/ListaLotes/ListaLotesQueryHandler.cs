using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Lotes;
using KProject.Application.Interfaces.Produtos;
using KProject.Common;

namespace KProject.Application.Lotes.ListaLotes;

public class ListaLotesQueryHandler(ILoteRepository lotes, IProdutoRepository produtos)
    : IQueryHandler<ListaLotesQuery, IReadOnlyList<LoteResponse>>
{
    public async Task<Result<IReadOnlyList<LoteResponse>>> Handle(ListaLotesQuery query, CancellationToken token)
    {
        if (!await produtos.ExistsAsync(query.ProdutoId, token))
            return Result.Failure<IReadOnlyList<LoteResponse>>(
                Error.NotFound("Produto.NaoEncontrado", $"Produto com ID {query.ProdutoId} não encontrado"));

        var result = await lotes.GetByProdutoIdAsync(query.ProdutoId, token);
        return Result.Success(result);
    }
}
