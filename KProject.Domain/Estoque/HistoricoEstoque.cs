namespace KProject.Domain.Estoque;

/// <summary>
/// Representa um evento imutável de movimentação do estoque.
/// Cada registro descreve uma alteração incremental (delta) aplicada ao estoque,
/// permitindo reconstruir o saldo atual a partir do histórico completo.
/// </summary>
public sealed class HistoricoEstoque
{
    public int Id { get; set; }
    public int EstoqueId { get; set; }

    /// <summary>
    /// Classificação semântica da movimentação do estoque.
    /// Utilizada para auditoria, rastreabilidade e relatórios.
    /// </summary>
    public TipoHistorico Tipo { get; set; }
    
    
    public DateTime CriadoEm { get; set; }

    /// <summary>
    /// Variação aplicada ao estoque neste evento.
    /// Valores positivos representam entrada de unidades;
    /// valores negativos representam saída de unidades.
    /// </summary>
    /// <remarks>
    /// O estoque não armazena o saldo atual.
    /// O valor disponível é obtido pela soma de todos os <see cref="DeltaQuantidade"/>
    /// registrados no histórico, em ordem cronológica.
    /// </remarks>
    public int DeltaQuantidade { get; set; }
}