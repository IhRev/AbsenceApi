using Absence.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Absence.Api.Services;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogCritical(exception, "Exception for user: {User}", httpContext.User?.FindFirst(ClaimTypes.Name));

        if (exception is AccessDeniedException e)
        {
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"
            });
        }
        else
        {
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            });
        }

        return true;
    }
}