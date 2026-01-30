namespace KProject.Domain.Estoque;

public class Estoque
{
    public int Id { get; private set; }
    public int LoteId { get; private set; }
    public int QuantidadeTotal => Historico.Sum(h => h.DeltaQuantidade);
    public List<HistoricoEstoque> Historico { get; private set; } = [];
}