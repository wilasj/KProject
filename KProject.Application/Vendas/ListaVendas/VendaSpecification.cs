using KProject.Application.Shared;
using KProject.Domain.Vendas;

namespace KProject.Application.Vendas.ListaVendas;

public class VendaSpecification : Specification<Venda>
{
    public VendaSpecification(string? busca)
    {
        if (!string.IsNullOrEmpty(busca))
            Criteria = v => v.Cliente!.Nome.ToLower().Contains(busca.ToLower());

        OrderBy = v => v.CriadaEm;
        Ascending = false;
    }
}
