using KProject.Common;

namespace KProject.Application.Interfaces;

public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken token);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken token);
}