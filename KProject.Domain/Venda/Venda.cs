
namespace KProject.Domain.Venda;

public sealed class Venda
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public StatusVenda Status { get; private set; }
    public List<ItemConsignado> Itens { get; private set; } = [];
}
