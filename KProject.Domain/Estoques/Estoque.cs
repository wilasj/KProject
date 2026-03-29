using KProject.Common;
using KProject.Domain.Vendas;

namespace KProject.Domain.Estoques;

public class Estoque
{
    public int Id { get; private set; }
    public int LoteId { get; private set; }
    public int QuantidadeAtual { get; private set; }
    private readonly List<HistoricoEstoque> _historico = [];
    public IReadOnlyCollection<HistoricoEstoque> Historico => _historico;

    private static readonly HashSet<TipoHistorico> TiposPositivos =
    [
        TipoHistorico.Entrada,
        TipoHistorico.RetornoConsignacao,
        TipoHistorico.AjusteEntrada,
    ];
    
    private Estoque() { }

    internal Estoque(uint quantidadeInicial, int? criadoPor = null)
    {
        if (quantidadeInicial > 0)
        {
            AplicarMovimento(quantidadeInicial, TipoHistorico.Entrada, criadoPor: criadoPor);
        }
    }

    public Result AplicarMovimento(uint quantidade, TipoHistorico tipo, Venda? venda = null, int? criadoPor = null)
    {
        if (quantidade == 0)
        {
            return Result.Failure(Error.Failure("Estoque.QuantidadeInvalida", "A quantidade não pode ser zero"));
        }

        var delta = TiposPositivos.Contains(tipo) ? (int)quantidade : -(int)quantidade;

        if (QuantidadeAtual + delta < 0)
        {
            return Result.Failure(Error.Failure("Estoque.EstoqueInsuficiente", "A movimentação resultaria em estoque negativo"));
        }

        QuantidadeAtual += delta;

        _historico.Add(new HistoricoEstoque
        {
            EstoqueId = Id,
            DeltaQuantidade = delta,
            Tipo = tipo,
            CriadoEm = DateTime.UtcNow,
            CriadoPor = criadoPor,
            Venda = venda,
        });

        return Result.Success();
    }
}