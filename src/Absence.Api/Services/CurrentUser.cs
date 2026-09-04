using Absence.Infrastructure.Identity;
using Absence.Api.Common.Exceptions;
using Absence.Api.Common.Interfaces;
using System.Security.Claims;

namespace Absence.Api.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string Id =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
        throw new MissingUserClaimException(ClaimTypes.NameIdentifier);

    public int ShortId =>
        int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(CustomClaimTypes.ShortId), out var shortId)
            ? shortId
            : throw new MissingUserClaimException(CustomClaimTypes.ShortId);
}