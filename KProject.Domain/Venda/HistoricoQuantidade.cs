namespace KProject.Domain.Venda;

public sealed record HistoricoQuantidade
{
    public uint Devolvido { get; set; }
    public uint Vendido { get; set; }
    public DateTime AlteradoEm { get; set; }
    public int AlteradoPor { get; set; }
}