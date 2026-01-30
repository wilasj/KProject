
namespace KProject.Domain.Venda;

public sealed class ItemConsignado
{
    public int Id { get; private set; }
    public int LoteId { get; private set; }
    public List<HistoricoQuantidade> Historico { get; set; } = [];
    // public uint QuantidadeTotal { get; set; }
    public uint Vendido => Historico.OrderBy(hq => hq.AlteradoEm).FirstOrDefault()?.Vendido ?? 0;
    public uint Devolvido => Historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.Devolvido ?? 0;
    public DateTime? UltimaAlteracao => Historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.AlteradoEm;
}