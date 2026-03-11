using KProject.Application.Clientes.ListaClientes;
using KProject.Application.Shared;
using KProject.Domain.Clientes;

namespace KProject.Application.Interfaces.Clientes;

public interface IClienteRepository
{
    Task AddAsync(Cliente cliente, CancellationToken token = default);
    Task<Page<ClienteResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default);
}
