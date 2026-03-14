using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;
using KProject.Domain.Vendas;

namespace KProject.Application.Interfaces.Vendas;

public interface IVendaRepository
{
    Task AddAsync(Venda venda, CancellationToken token = default);
    Task<Page<VendaResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default);
}
