using FluentValidation;
using FluentValidation.Results;
using KProject.Application;
using KProject.Application.Interfaces;
using KProject.Common;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace KProject.Tests;

public class DummyCommand : ICommand;

public class ValidationCommandHandlerTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICommandHandler<DummyCommand> _handler;

    public ValidationCommandHandlerTests()
    {
        _handler = Substitute.For<ICommandHandler<DummyCommand>>();
        _serviceProvider = Substitute.For<IServiceProvider>();
    }

    [Fact]
    public async Task CommandHandler_SemValidator_DeveChamarOHandlerDireto()
    {
        _handler
            .Handle(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        
        _serviceProvider.GetService(typeof(IValidator<DummyCommand>)).ReturnsNull();

        var vHandler = new ValidationCommandHandler<DummyCommand>(_serviceProvider, _handler);

        var dummyCommand = new DummyCommand();

        var result = await vHandler.Handle(dummyCommand, TestContext.Current.CancellationToken);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        await _handler.Received().Handle(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandHandler_ComValidator_PassandoValidacao_ChamaHandlerInterno()
    {
        _handler
            .Handle(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var validator = Substitute.For<IValidator<DummyCommand>>();
        
        validator
            .ValidateAsync(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _serviceProvider.GetService(typeof(IValidator<DummyCommand>)).Returns(validator);

        var vHandler = new ValidationCommandHandler<DummyCommand>(_serviceProvider, _handler);
        
        var dummyCommand = new DummyCommand();

        var result = await vHandler.Handle(dummyCommand, TestContext.Current.CancellationToken);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        
        await validator.Received().ValidateAsync(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>());
        await _handler.Received().Handle(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommandHandler_ComValidator_FalhandoValidacao_NaoChamaHandlerInterno_ERetornaErros()
    {
        var validator = Substitute.For<IValidator<DummyCommand>>();

        validator
            .ValidateAsync(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new List<ValidationFailure>
            {
                new("Campo", "Mensagem") { ErrorCode = "Codigo" }
            }));

        _serviceProvider.GetService(typeof(IValidator<DummyCommand>)).Returns(validator);

        var vHandler = new ValidationCommandHandler<DummyCommand>(_serviceProvider, _handler);

        var dummyCommand = new DummyCommand();

        var result = await vHandler.Handle(dummyCommand, TestContext.Current.CancellationToken);

        result.Errors.ShouldNotBeEmpty();
        result.IsSuccess.ShouldBeFalse();

        await validator.Received().ValidateAsync(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>());
        await _handler.DidNotReceive().Handle(Arg.Any<DummyCommand>(), Arg.Any<CancellationToken>());
    }
}