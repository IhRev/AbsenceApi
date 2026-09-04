using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Absence.Api.Features.Invitations;

public static class GetUserInvitations
{
    public sealed class Query : IRequest<IEnumerable<InvitationDTO>>;

    internal sealed class Handler(
        IUser user,
        AbsenceContext db
    ) : IRequestHandler<Query, IEnumerable<InvitationDTO>>
    {
        public async Task<IEnumerable<InvitationDTO>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.OrganizationUserInvitations
                .Where(_ => _.Invited == user.ShortId)
                .Select(_ => new InvitationDTO
                {
                    Id = _.Id,
                    Organization = _.Organization.Name,
                    Inviter = _.InviterUser.Email!
                })
                .ToListAsync(cancellationToken);
        }
    }
}
