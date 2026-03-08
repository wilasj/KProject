using KProject.Common;
using KProject.Domain.Estoques;

namespace KProject.Domain.Lotes;

public sealed class Lote
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public int Numero { get; private set; }
    public DateOnly Validade { get; private set; }
    public Estoque Estoque { get; private set; } = null!;

    private Lote() { }

    private Lote(int produtoId, int numero, DateOnly validade, uint quantidadeInicial)
    {
        ProdutoId = produtoId;
        Numero = numero;
        Validade = validade;
        Estoque = new Estoque(quantidadeInicial);
    }

    public static Result<Lote> Criar(int produtoId, int numero, DateOnly validade, uint quantidadeInicial = 0)
    {
        if (produtoId <= 0)
            return Result.Failure<Lote>(Error.Failure("Lote.ProdutoInvalido", "O ID do produto deve ser maior que zero"));

        if (numero <= 0)
            return Result.Failure<Lote>(Error.Failure("Lote.NumeroInvalido", "O número do lote deve ser maior que zero"));

        return Result.Success(new Lote(produtoId, numero, validade, quantidadeInicial));
    }
}