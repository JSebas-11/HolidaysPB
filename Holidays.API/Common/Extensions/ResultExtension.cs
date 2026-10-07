using HolidaysPB.Core.Common.Enums;
using HolidaysPB.Core.Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysPB.Api.Common.Extensions;

internal static class ResultExtensions {
    internal static IActionResult ToApiResult<T>(this Result<T> result, Func<T, IActionResult> onSuccess)
        => result.IsSuccess ? onSuccess(result.Value!) : result.Error!.ToProblem();
    internal static IActionResult ToApiResult(this Result result, IActionResult? onSuccess = null)
        => result.IsSuccess ? onSuccess ?? new NoContentResult() : result.Error!.ToProblem();

    // INNER METHS
    private static ObjectResult ToProblem(this AppError error) {
        var problemDetails = new ProblemDetails {
            Status = error.Kind.ToStatusCode(), Title = error.Kind.ToTitle(), Detail = error.Message
        };

        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    private static int ToStatusCode(this ErrorKind error)
        => error switch {
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            ErrorKind.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    private static string ToTitle(this ErrorKind error)
        => error switch {
            ErrorKind.NotFound => "Not found resource.",
            ErrorKind.Validation => "Validation failed.",
            ErrorKind.Conflict => "Entities/resources conflict.",
            ErrorKind.Unauthorized => "Unauthorized access.",
            ErrorKind.Forbidden => "Forbidden resource.",
            ErrorKind.Unexpected => "Internal unexpected error ocurred.",
            _ => "An error ocurred."
        };
}