namespace KProject.Domain.Venda;

public sealed record HistoricoQuantidade
{
    public int Id { get; set; }
    public int ItemConsignadoId { get; set; }
    public uint Devolvido { get; set; }
    public uint Vendido { get; set; }
    public DateTime AlteradoEm { get; set; }
    public int AlteradoPor { get; set; }
}