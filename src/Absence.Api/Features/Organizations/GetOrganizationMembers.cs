using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class GetOrganizationMembers
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>>
    {
        public async Task<OneOf<Success<IEnumerable<MemberDTO>>, BadRequest>> Handle(Query request, CancellationToken cancellationToken)
        {
            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == request.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"No organization with id {request.OrganizationId} found.");
            }

            var organizationUsers = await db.OrganizationUsers
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .Select(_ => new MemberDTO
                {
                    Id = _.UserId,
                    IsAdmin = _.IsAdmin,
                    IsOwner = _.UserId == _.Organization.OwnerId,
                    FullName = _.User.FirstName + " " + _.User.LastName
                })
                .ToListAsync(cancellationToken);

            return new Success<IEnumerable<MemberDTO>>(organizationUsers);
        }
    }
}
