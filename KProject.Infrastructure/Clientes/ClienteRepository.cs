using KProject.Application.Clientes.ListaClientes;
using KProject.Application.Interfaces.Clientes;
using KProject.Application.Shared;
using KProject.Domain.Clientes;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Clientes;

public class ClienteRepository(AppDbContext db) : IClienteRepository
{
    public async Task AddAsync(Cliente cliente, CancellationToken token = default) =>
        await db.Clientes.AddAsync(cliente, token);

    public async Task<Page<ClienteResponse>> GetPagedAsync(string? busca, int page, int pageSize, CancellationToken token = default)
    {
        var spec = new ClienteSpecification(busca);
        var q = db.Clientes.AsQueryable();

        if (spec.Criteria != null)
            q = q.Where(spec.Criteria);

        if (spec.OrderBy != null)
            q = spec.Ascending
                ? q.OrderBy(spec.OrderBy)
                : q.OrderByDescending(spec.OrderBy);

        var total = await q.CountAsync(token);

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClienteResponse(c.Id, c.Nome))
            .ToListAsync(token);

        return new Page<ClienteResponse>(items, total);
    }
}
