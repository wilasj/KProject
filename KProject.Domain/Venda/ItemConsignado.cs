
using KProject.Common;

namespace KProject.Domain.Venda;

public sealed class ItemConsignado
{
    public int Id { get; private set; }
    public int VendaId { get; private set; }
    public int LoteId { get; private set; }
    private readonly List<HistoricoQuantidade> _historico = [];
    public IReadOnlyList<HistoricoQuantidade> Historico => _historico;
    public uint QuantidadeConsignada { get; private set; }
    public uint Vendido => _historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.Vendido ?? 0;
    public uint Devolvido => _historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.Devolvido ?? 0;
    public uint EmAberto => QuantidadeConsignada - Vendido - Devolvido;
    
    public DateTime? UltimaAlteracao => _historico.OrderBy(hq => hq.AlteradoEm).LastOrDefault()?.AlteradoEm;

    private ItemConsignado()
    {
    }
    
    private ItemConsignado(int loteId, int usuarioId, uint quantidadeConsignada)
    {
        LoteId = loteId;
        QuantidadeConsignada = quantidadeConsignada;
        _historico.Add(new HistoricoQuantidade { AlteradoEm = DateTime.UtcNow, AlteradoPor = usuarioId, Vendido = 0, Devolvido = 0 });
    }

    public static Result<ItemConsignado> Criar(int loteId, int usuarioId, uint quantidadeConsignada)
    {
        if (quantidadeConsignada <= 0)
        {
            return Result.Failure<ItemConsignado>(Error.Failure("ItemConsignado.QuantidadeInvalida", "A quantidade deve ser maior que zero"));
        }

        if (loteId <= 0)
        {
            return Result.Failure<ItemConsignado>(Error.Failure("ItemConsignado.LoteInvalido", "Lote inválido"));
        }
        
        var item = new ItemConsignado(loteId, usuarioId, quantidadeConsignada);
        
        return Result.Success(item);
    }
}