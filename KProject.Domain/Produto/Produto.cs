namespace KProject.Domain.Produto;

public sealed class Produto
{
    public int Id { get; private set; }
    public string Codigo { get; private set; }
    public string Descricao { get; private set; }
    public string CodigoAnvisa { get; private set; }
}