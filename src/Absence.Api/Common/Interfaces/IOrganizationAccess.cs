using Absence.Api.Common.Results;
using Absence.Infrastructure.Entities;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Common.Interfaces;

/// <summary>
/// Single source of truth for what a caller may do inside an organization.
/// Callers who are not members get <see cref="NotFound"/> so that organization
/// ids are not confirmed to strangers; members lacking rights get <see cref="AccessDenied"/>.
/// </summary>
public interface IOrganizationAccess
{
    Task<OneOf<Success<OrganizationUserEntity>, NotFound, AccessDenied>> RequireMemberAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<OneOf<Success<OrganizationUserEntity>, NotFound, AccessDenied>> RequireAdminAsync(
        int organizationId,
        CancellationToken cancellationToken = default);
}
