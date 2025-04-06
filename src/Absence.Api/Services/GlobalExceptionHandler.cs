using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Absence.Api.Services;

public class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogCritical(exception, "Exception for user: {User}", httpContext.User?.FindFirst(ClaimTypes.Name));

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unexpected",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        });

        return true;
    }
}