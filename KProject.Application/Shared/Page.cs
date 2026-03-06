namespace KProject.Application.Shared;

public record Page<T>(IReadOnlyList<T> Items, int Total);
