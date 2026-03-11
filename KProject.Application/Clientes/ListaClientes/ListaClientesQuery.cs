using KProject.Application.Shared;

namespace KProject.Application.Clientes.ListaClientes;

public record ListaClientesQuery : PagedQuery<ClienteResponse>
{
    public string? Busca { get; init; }
}
