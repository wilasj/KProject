using KProject.Application.Interfaces;

namespace KProject.Application.Produtos.CriaProduto;

public class CriaProdutoCommand: ICommand
{
    public required string Nome { get; init; }
    public required string Referencia { get; init; }
    public required string Descricao { get; init; }
    public required string CodigoAnvisa { get; init; }
}