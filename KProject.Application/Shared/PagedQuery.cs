using KProject.Application.Interfaces;

namespace KProject.Application.Shared;

public abstract record PagedQuery<TResponse> : IQuery<Page<TResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
