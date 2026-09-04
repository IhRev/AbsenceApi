using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Invitations;

public static class AcceptInvitation
{
    public sealed class Command(int id, bool accespted) : IRequest<OneOf<Success, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
        public bool Accespted { get; } = accespted;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var invitation = await db.OrganizationUserInvitations.FirstOrDefaultAsync(
                _ => _.Id == request.Id,
                cancellationToken);
            if (invitation is null)
            {
                return new NotFound();
            }

            if (invitation.Invited != user.ShortId)
            {
                return new AccessDenied();
            }

            if (request.Accespted)
            {
                var existingMembership = await db.OrganizationUsers.FirstOrDefaultAsync(
                    _ => _.OrganizationId == invitation.OrganizationId && _.UserId == user.ShortId,
                    cancellationToken);
                if (existingMembership is null)
                {
                    db.OrganizationUsers.Add(new OrganizationUserEntity
                    {
                        OrganizationId = invitation.OrganizationId,
                        UserId = invitation.Invited,
                        IsAdmin = false
                    });
                }
            }

            db.OrganizationUserInvitations.Remove(invitation);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
