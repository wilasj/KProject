namespace KProject.Application.Lotes.HistoricoLote;

public record HistoricoEstoqueResponse(
    int Id,
    string Tipo,
    int DeltaQuantidade,
    DateTime CriadoEm,
    int? VendaId);
