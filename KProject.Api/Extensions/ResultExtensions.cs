using KProject.Common;

namespace KProject.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        var errors = result.Errors.Select(e => new { e.Code, e.Description });

        return result.Errors.First().Type switch
        {
            ErrorType.Failure => TypedResults.InternalServerError(errors),
            ErrorType.Validation => TypedResults.BadRequest(errors),
            ErrorType.Problem => TypedResults.InternalServerError(errors),
            ErrorType.NotFound => TypedResults.NotFound(errors),
            ErrorType.Conflict => TypedResults.Conflict(errors),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}