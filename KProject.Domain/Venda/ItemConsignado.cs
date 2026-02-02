
namespace KProject.Domain.Venda;

public sealed class ItemConsignado
{
    public int Id { get; private set; }
    public int LoteId { get; private set; }
    private readonly List<HistoricoQuantidade> _historico = [];
    public IReadOnlyList<HistoricoQuantidade> Historico => _historico;
    public uint QuantidadeConsignada { get; private set; }
    public uint Vendido => _historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.Vendido ?? 0;
    public uint Devolvido => _historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.Devolvido ?? 0;
    public uint EmAberto => QuantidadeConsignada - Vendido - Devolvido;
    
    public DateTime? UltimaAlteracao => _historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.AlteradoEm;
}