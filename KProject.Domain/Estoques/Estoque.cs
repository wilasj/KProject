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
    
    private Estoque(int loteId)
    {
        LoteId = loteId;
    }

    public static Result<Estoque> Criar(int loteId, uint quantidadeInicial = 0)
    {
        if (loteId <= 0)
            return Result.Failure<Estoque>(Error.Failure("Estoque.LoteInvalido", "O ID do lote deve ser maior que zero"));

        var estoque = new Estoque(loteId);

        if (quantidadeInicial <= 0)
        {
            return Result.Success(estoque);
        }

        var movimento = estoque.AplicarMovimento(quantidadeInicial, TipoHistorico.Entrada);
        
        return movimento.IsFailure ? Result.Failure<Estoque>(movimento.Errors.First()) : Result.Success(estoque);
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