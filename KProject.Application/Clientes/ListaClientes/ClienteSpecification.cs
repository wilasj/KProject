using KProject.Application.Shared;
using KProject.Domain.Clientes;

namespace KProject.Application.Clientes.ListaClientes;

public class ClienteSpecification : Specification<Cliente>
{
    public ClienteSpecification(string? busca)
    {
        if (!string.IsNullOrEmpty(busca))
            Criteria = c => c.Nome.ToLower().Contains(busca.ToLower());

        OrderBy = c => c.Nome;
    }
}
