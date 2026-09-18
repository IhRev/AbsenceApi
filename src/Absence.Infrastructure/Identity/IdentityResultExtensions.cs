using Microsoft.AspNetCore.Identity;

namespace Absence.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    /// <summary>
    /// For Identity writes that have already passed validation. A failure at that point is a
    /// concurrency or store problem rather than a user error, so it must surface instead of
    /// being discarded and reported to the caller as success.
    /// </summary>
    public static void EnsureSucceeded(this IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(_ => _.Description));
        throw new InvalidOperationException($"{operation} failed: {errors}");
    }
}