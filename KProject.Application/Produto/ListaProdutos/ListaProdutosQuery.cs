using KProject.Application.Shared;

namespace KProject.Application.Produto.ListaProdutos;

public record ListaProdutosQuery : PagedQuery<ProdutoResponse>
{
    public string? Busca { get; init; }
}