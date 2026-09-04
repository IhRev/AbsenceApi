using Absence.Api.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Absence.Api.Services;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var unauthorized = exception is MissingUserClaimException;
        var user = httpContext.User?.FindFirst(ClaimTypes.Name);

        if (unauthorized)
        {
            logger.LogWarning(exception, "Rejected request for user: {User}", user);
        }
        else
        {
            logger.LogCritical(exception, "Exception for user: {User}", user);
        }

        var statusCode = unauthorized
            ? StatusCodes.Status401Unauthorized
            : StatusCodes.Status500InternalServerError;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = unauthorized ? "Unauthorized" : "Unexpected",
                Type = unauthorized
                    ? "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"
                    : "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            },
            cancellationToken);

        return true;
    }
}
