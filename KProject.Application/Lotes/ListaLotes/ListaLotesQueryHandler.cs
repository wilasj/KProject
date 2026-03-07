using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Lotes.ListaLotes;

public class ListaLotesQueryHandler(AppDbContext context)
    : IQueryHandler<ListaLotesQuery, IReadOnlyList<LoteResponse>>
{
    public async Task<Result<IReadOnlyList<LoteResponse>>> Handle(
        ListaLotesQuery query,
        CancellationToken token)
    {
        var produtoExiste = await context.Produtos
            .AnyAsync(p => p.Id == query.ProdutoId, token);

        if (!produtoExiste)
            return Result.Failure<IReadOnlyList<LoteResponse>>(
                Error.NotFound("Produto.NaoEncontrado", $"Produto com ID {query.ProdutoId} não encontrado"));

        var lotes = await context.Lotes
            .Where(l => l.ProdutoId == query.ProdutoId)
            .OrderBy(l => l.Validade)
            .Select(l => new LoteResponse(l.Id, l.Numero, l.Validade, l.Estoque.QuantidadeAtual))
            .ToListAsync(token);

        return Result.Success<IReadOnlyList<LoteResponse>>(lotes);
    }
}
