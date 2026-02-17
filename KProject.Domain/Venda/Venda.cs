using KProject.Common;

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

    private Venda(int clienteId, int criadaPor)
    {
        ClienteId = clienteId;
        CriadaPor = criadaPor;
        CriadaEm = DateTime.UtcNow;
        Status = StatusVenda.Aberta;
    }

    //TODO: A melhor forma de fazer isso eh realmente usando um int pro loteId? Nao daria pra gente usar uma tupla pra especificar mais essa variavel?
    public static Result<Venda> Criar(int clienteId, int criadaPor, Dictionary<int, uint> novosItens)
    {
        if (clienteId == 0)
        {
            return Result.Failure<Venda>(Error.Failure("Venda.ClienteInvalido", "O ID do cliente deve ser maior que zero"));
        }
        
        if (novosItens.Count == 0)
        {
            return Result.Failure<Venda>(Error.Failure("Venda.ItensInvalidos", "Nenhum item foi fornecido para a venda"));
        }
        
        var venda = new Venda(clienteId, criadaPor);

        var addResult = venda.AdicionarItens(novosItens, venda.CriadaPor);
        
        return addResult.IsFailure ? Result.Failure<Venda>(addResult.Errors.First()) : Result.Success(venda);
    }

    private Result AdicionarItens(Dictionary<int, uint> novosItens, int criadoPor)
    {
        foreach (var (loteId, quantidadeConsignada) in novosItens)
        {
            var itemResult = ItemConsignado.Criar(loteId, criadoPor, quantidadeConsignada);
            
            if (itemResult.IsFailure)
            {
                return Result.Failure(itemResult.Errors.First());
            }

            _itens.Add(itemResult.Value);
        }

        return Result.Success();
    }
    
    public Result AdicionarItem(ItemConsignado item)
    {
        if (Status is StatusVenda.Fechada or StatusVenda.Cancelada)
        {
            return Result.Failure(Error.Failure("Venda.StatusInvalido", "A venda não pode ser alterada quando status é cancelado/fechado"));
        }

        if (_itens.Any(i => i.LoteId == item.LoteId))
        {
            return Result.Failure(Error.Failure("Venda.ItemDuplicado", "O item já existe na venda"));
        }
            
        _itens.Add(item);

        return Result.Success();
    }

    //TODO: Por enquanto, so faz verificacoes simples e fecha a venda. Na verdade, depende de ItemConsignado pra atualizar as quantidades em aberto
    // para devolvidos e setar o status.
    public Result FecharVenda()
    {
        if (Status is StatusVenda.Fechada or StatusVenda.Cancelada)
        {
            return Result.Failure(Error.Failure("Venda.FechamentoInvalido", "É impossível fechar vendas já fechadas/canceladas"));
        }
        
        Status = StatusVenda.Fechada;

        return Result.Success();
    }

    public Result CancelarVenda()
    {
        if (Status is StatusVenda.Fechada or StatusVenda.Cancelada)
        {
            return Result.Failure(Error.Failure("Venda.CancelamentoInvalido", "É impossível cancelar vendas já fechadas/canceladas"));
        }

        Status = StatusVenda.Cancelada;

        return Result.Success();
    }
}
