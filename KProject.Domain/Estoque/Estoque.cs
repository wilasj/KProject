namespace KProject.Domain.Estoque;

public class Estoque
{
    public int Id { get; private set; }
    public int LoteId { get; private set; }
    private readonly List<HistoricoEstoque> _historico = [];
    public IReadOnlyCollection<HistoricoEstoque> Historico => _historico;
    public int QuantidadeTotal => _historico.Sum(h => h.DeltaQuantidade);
}