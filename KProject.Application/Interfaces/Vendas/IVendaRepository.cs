using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;
using KProject.Application.Vendas.ObtemVenda;
using KProject.Domain.Vendas;

namespace KProject.Application.Interfaces.Vendas;

public interface IVendaRepository
{
    Task AddAsync(Venda venda, CancellationToken token = default);
    Task<Page<VendaResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default);
    Task<VendaDetalheResponse?> GetDetalheAsync(int id, CancellationToken token = default);
    Task<Venda?> GetByIdWithItensAsync(int id, CancellationToken token = default);
}
