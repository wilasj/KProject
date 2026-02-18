using FluentValidation;
using KProject.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .Scan(s => s
                .FromAssembliesOf(typeof(ICommand))
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandler<>));
        
        return services;
    }
}