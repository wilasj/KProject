namespace KProject.Domain.Produto;

public sealed class Produto
{
    public int Id { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public string Nome { get; private set; }
    public string Referencia { get; private set; }
    public string Descricao { get; private set; }
    public string CodigoAnvisa { get; private set; }
}