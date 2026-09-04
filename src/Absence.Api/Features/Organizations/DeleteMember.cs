using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class DeleteMember
{
    public sealed class Command(int organizationId, int memberId) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
        public int MemberId { get; } = memberId;
    }

    internal sealed class Handler(
        IUser user,
        AbsenceContext db
    ) : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
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
                _ => _.OrganizationId == request.OrganizationId && _.UserId == request.MemberId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"User with id {request.MemberId} doesn't belong to organization.");
            }
            if (organizationUser.UserId == organization.OwnerId)
            {
                return new BadRequest("Cannot remove the organization owner.");
            }

            db.OrganizationUsers.Remove(organizationUser);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
