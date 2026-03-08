using KProject.Application.Produtos.ListaProdutos;
using KProject.Application.Shared;
using KProject.Domain.Produtos;

namespace KProject.Application.Interfaces.Produtos;

public interface IProdutoRepository
{
    Task AddAsync(Produto produto, CancellationToken token = default);
    Task<bool> ExistsAsync(int id, CancellationToken token = default);
    Task<Page<ProdutoResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default);
}
