namespace KProject.Application.Lotes.HistoricoLote;

public record HistoricoPage(
    IReadOnlyList<HistoricoEstoqueResponse> Items,
    bool HasMore);
