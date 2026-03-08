using KProject.Application.Interfaces.Convites;
using KProject.Domain.Convites;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Convites;

public class ConviteRepository(AppDbContext db) : IConviteRepository
{
    public Task<Convite?> FindByUsuarioIdAsync(int usuarioId, CancellationToken token = default) =>
        db.Convites.FirstOrDefaultAsync(c => c.CriadoPorId == usuarioId && c.UsadoEm == null, token);

    public Task<Convite?> FindByTokenAsync(string conviteToken, CancellationToken token = default) =>
        db.Convites.FirstOrDefaultAsync(c => c.Token == conviteToken, token);

    public async Task AddAsync(Convite convite, CancellationToken token = default) =>
        await db.Convites.AddAsync(convite, token);
}
