namespace KProject.Domain.Lote;

public sealed class Lote
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public int Numero { get; private set; }
    public DateOnly Validade { get; private set; }
}