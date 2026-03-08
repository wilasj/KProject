using KProject.Application.Interfaces;

namespace KProject.Application.Lotes.HistoricoLote;

public record HistoricoLoteQuery(
    int LoteId,
    int Pagina,
    int TamanhoPagina) : IQuery<HistoricoPage>;
