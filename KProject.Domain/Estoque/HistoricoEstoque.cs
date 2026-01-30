namespace KProject.Domain.Estoque;

public sealed class HistoricoEstoque
{
    public int Id { get; set; }
    public int EstoqueId { get; set; }
    public TipoHistorico Tipo { get; set; }
    public DateTime CriadoEm { get; set; }
    public int DeltaQuantidade { get; set; }
    public int? ReferenciaId { get; set; }
    //usuario
}