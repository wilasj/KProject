using KProject.Common;

namespace KProject.Domain.Produto;

public sealed class Produto
{
    public int Id { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public string Nome { get; private set; }
    public string Referencia { get; private set; }
    public string Descricao { get; private set; }
    public string CodigoAnvisa { get; private set; }

    private Produto(string nome, string referencia, string descricao, string codigoAnvisa)
    {
        Nome = nome;
        Referencia = referencia;
        Descricao = descricao;
        CodigoAnvisa = codigoAnvisa;
        CriadoEm = DateTime.UtcNow;
    }
    
    public static Result<Produto> Criar(string nome, string referencia, string descricao, string codigoAnvisa)
    {
        if (string.IsNullOrEmpty(nome))
        {
            return Result.Failure<Produto>(Error.Failure("Produto.NomeVazio", "O nome do produto não pode ser vazio"));
        }

        if (string.IsNullOrEmpty(referencia))
        {
            return Result.Failure<Produto>(Error.Failure("Produto.ReferenciaVazia", "A referência do produto não pode ser vazia"));
        }

        if (string.IsNullOrEmpty(descricao))
        {
            return Result.Failure<Produto>(Error.Failure("Produto.DescricaoVazia", "A descrição do produto não pode ser vazia"));
        }

        if (string.IsNullOrEmpty(codigoAnvisa))
        {
            return Result.Failure<Produto>(Error.Failure("Produto.CodigoVazio", "O código do produto não pode ser vazio"));
        }
        
        //TODO: Provavelmente teremos mais validacoes aqui como tamanho do codigo e referencia.
        
        var produto = new Produto(nome, referencia, descricao, codigoAnvisa);
        
        return Result.Success(produto);       
    }
}

