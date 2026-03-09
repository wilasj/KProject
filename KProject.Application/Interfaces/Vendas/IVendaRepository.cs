using KProject.Application.Shared;
using KProject.Application.Vendas.ListaVendas;

namespace KProject.Application.Interfaces.Vendas;

public interface IVendaRepository
{
    Task<Page<VendaResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default);
}
