using System.Diagnostics.CodeAnalysis;

namespace KProject.Common;

public class Result
{
    public Result(bool isSuccess, IReadOnlyCollection<Error> error)
    {
        if (isSuccess && error.Count != 0||
            !isSuccess && error.Count == 0)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Errors = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyCollection<Error> Errors { get; }

    public static Result Success() => new(true, []);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, []);

    public static Result Failure(Error error) => new(false, [error]);

    public static Result Failure(IReadOnlyCollection<Error> errors) => new(false, errors);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, [error]);

    public static Result<TValue> Failure<TValue>(IReadOnlyCollection<Error> errors) => new(default, false, errors);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, IReadOnlyCollection<Error> errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}