using KProject.Common;

namespace KProject.Application.Interfaces;

public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken token);
}