using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class ChangeMemberAccess
{
    public sealed class Command(int organizationId, int userId, bool isAdmin) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public int OrganizationId { get; } = organizationId;
        public int UserId { get; } = userId;
        public bool IsAdmin { get; } = isAdmin;
    }

    internal sealed class Handler(
        IUser user,
        AbsenceContext db
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organizationOwner = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.OrganizationId == request.OrganizationId && _.UserId == user.ShortId,
                cancellationToken);
            if (organizationOwner is null)
            {
                return new NotFound();
            }
            if (!organizationOwner.IsAdmin)
            {
                return new AccessDenied();
            }

            var organization = await db.Organizations.FirstOrDefaultAsync(
                _ => _.Id == request.OrganizationId,
                cancellationToken);
            if (organization is null)
            {
                return new NotFound();
            }

            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.OrganizationId == request.OrganizationId && _.UserId == request.UserId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"User with id {request.UserId} doesn't belong to organization.");
            }
            if (organizationUser.UserId == organization.OwnerId)
            {
                return new BadRequest("Cannot change the organization owner's access.");
            }

            if (organizationUser.IsAdmin == request.IsAdmin)
            {
                return new BadRequest("Cannot change access to the same.");
            }

            organizationUser.IsAdmin = request.IsAdmin;
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
