namespace KProject.Application.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken token = default);
}
