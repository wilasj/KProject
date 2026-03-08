using KProject.Domain.Convites;

namespace KProject.Application.Interfaces.Convites;

public interface IConviteRepository
{
    Task<Convite?> FindByUsuarioIdAsync(int usuarioId, CancellationToken token = default);
    Task<Convite?> FindByTokenAsync(string conviteToken, CancellationToken token = default);
    Task AddAsync(Convite convite, CancellationToken token = default);
}
