using KProject.Common;

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

    internal Estoque(uint quantidadeInicial)
    {
        if (quantidadeInicial > 0)
            AplicarMovimento(quantidadeInicial, TipoHistorico.Entrada);
    }

    public Result AplicarMovimento(uint quantidade, TipoHistorico tipo)
    {
        if (quantidade == 0)
            return Result.Failure(Error.Failure("Estoque.QuantidadeInvalida", "A quantidade não pode ser zero"));

        var delta = TiposPositivos.Contains(tipo) ? (int)quantidade : -(int)quantidade;

        if (QuantidadeAtual + delta < 0)
            return Result.Failure(Error.Failure("Estoque.EstoqueInsuficiente", "A movimentação resultaria em estoque negativo"));

        QuantidadeAtual += delta;
        
        _historico.Add(new HistoricoEstoque
        {
            EstoqueId = Id,
            DeltaQuantidade = delta,
            Tipo = tipo,
            CriadoEm = DateTime.UtcNow
        });

        return Result.Success();
    }
}