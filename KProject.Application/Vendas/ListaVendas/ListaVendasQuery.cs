using KProject.Application.Shared;

namespace KProject.Application.Vendas.ListaVendas;

public record ListaVendasQuery : PagedQuery<VendaResponse>
{
    public string? Busca { get; init; }
}
