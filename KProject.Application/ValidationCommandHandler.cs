using System.Collections.Immutable;
using FluentValidation;
using KProject.Application.Interfaces;
using KProject.Common;
using Microsoft.Extensions.DependencyInjection;

namespace KProject.Application;

public class ValidationCommandHandler<TCommand>(
    IServiceProvider provider,
    ICommandHandler<TCommand> handler) : ICommandHandler<TCommand> where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken token)
    {
        var validator = provider.GetService<IValidator<TCommand>>();
        
        if (validator is null)
        {
            return await handler.Handle(command, token);
        }

        var result = await validator.ValidateAsync(command, token);

        if (result.IsValid)
        {
            return await handler.Handle(command, token);
        }

        var errors = result.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToImmutableList();

        return Result.Failure(errors);
    }
}