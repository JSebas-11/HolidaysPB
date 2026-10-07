using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysPB.Api.Common.ErrorHandling;

internal sealed class GlobalExceptionHandler : IExceptionHandler {
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger) {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken ct) {
        _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
        
        return await _problemDetails.TryWriteAsync(
            new ProblemDetailsContext {
                HttpContext = context,
                Exception = ex,
                ProblemDetails = new ProblemDetails {
                    Type = ex.GetType().Name,
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred",
                    Detail = "An unexpected error occurred. Please try again later."
                }
            }
        );
    }
}