
namespace KProject.Domain.Venda;

public sealed class Venda
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public int CriadaPor { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public StatusVenda Status { get; private set; }
    private readonly List<ItemConsignado> _itens = [];
    public IReadOnlyCollection<ItemConsignado> Itens => _itens;
    
    
}
