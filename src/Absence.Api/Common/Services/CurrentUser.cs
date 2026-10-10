using Absence.Infrastructure.Identity;
using Absence.Api.Common.Exceptions;
using Absence.Api.Common.Interfaces;
using System.Security.Claims;

namespace Absence.Api.Common.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string Id =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
        throw new MissingUserClaimException(ClaimTypes.NameIdentifier);

    public int ShortId =>
        int.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(CustomClaimTypes.ShortId), out var shortId)
            ? shortId
            : throw new MissingUserClaimException(CustomClaimTypes.ShortId);
}
