using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Common.Services;

internal class OrganizationAccess(AbsenceContext db, IUser user) : IOrganizationAccess
{
    public async Task<OneOf<OrganizationUserEntity, NotFound>> RequireMemberAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var membership = await GetMembershipAsync(organizationId, cancellationToken);
        if (membership is null)
        {
            return new NotFound();
        }

        return membership;
    }

    public async Task<OneOf<OrganizationUserEntity, NotFound, AccessDenied>> RequireAdminAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var membership = await GetMembershipAsync(organizationId, cancellationToken);
        if (membership is null)
        {
            return new NotFound();
        }
        if (!membership.IsAdmin)
        {
            return new AccessDenied();
        }

        return membership;
    }

    public async Task<OneOf<OrganizationEntity, NotFound, AccessDenied>> RequireOwnerAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var organization = await db.Organizations.FirstOrDefaultAsync(
            _ => _.Id == organizationId,
            cancellationToken);
        if (organization is null)
        {
            return new NotFound();
        }
        if (organization.OwnerId == user.ShortId)
        {
            return organization;
        }

        var membership = await GetMembershipAsync(organizationId, cancellationToken);
        if (membership is null)
        {
            return new NotFound();
        }

        return new AccessDenied();
    }

    private Task<OrganizationUserEntity?> GetMembershipAsync(
        int organizationId,
        CancellationToken cancellationToken) =>
        db.OrganizationUsers.FirstOrDefaultAsync(
            _ => _.UserId == user.ShortId && _.OrganizationId == organizationId,
            cancellationToken);
}
