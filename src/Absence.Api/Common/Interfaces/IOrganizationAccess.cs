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
    Task<OneOf<OrganizationUserEntity, NotFound>> RequireMemberAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<OneOf<OrganizationUserEntity, NotFound, AccessDenied>> RequireAdminAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the organization only for its owner. Non-owners who are members get
    /// <see cref="AccessDenied"/>; everyone else gets <see cref="NotFound"/>, since a
    /// non-member must not be able to tell an existing organization from a missing one.
    /// </summary>
    Task<OneOf<OrganizationEntity, NotFound, AccessDenied>> RequireOwnerAsync(
        int organizationId,
        CancellationToken cancellationToken = default);
}
