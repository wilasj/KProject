using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Lotes;
using KProject.Common;

namespace KProject.Application.Lotes.HistoricoLote;

public class HistoricoLoteQueryHandler(ILoteRepository lotes)
    : IQueryHandler<HistoricoLoteQuery, HistoricoPage>
{
    public async Task<Result<HistoricoPage>> Handle(HistoricoLoteQuery query, CancellationToken token)
    {
        if (!await lotes.ExistsAsync(query.LoteId, token))
            return Result.Failure<HistoricoPage>(
                Error.NotFound("Lote.NaoEncontrado", $"Lote com ID {query.LoteId} não encontrado"));

        var page = await lotes.GetHistoricoPagedAsync(query.LoteId, query.Pagina, query.TamanhoPagina, token);
        return Result.Success(page);
    }
}
