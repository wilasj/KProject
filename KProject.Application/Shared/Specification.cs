using System.Linq.Expressions;

namespace KProject.Application.Shared;

public abstract class Specification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }
    public Expression<Func<T, object>>? OrderBy { get; protected set; }
    public bool Ascending { get; protected set; } = true;
}
