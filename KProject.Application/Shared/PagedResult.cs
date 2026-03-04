namespace KProject.Application.Shared;

public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
