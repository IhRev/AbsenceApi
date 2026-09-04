using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public static class GetOrganizationMembers
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<MemberDTO>>, NotFound>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<MemberDTO>>, NotFound>>
    {
        public async Task<OneOf<Success<IEnumerable<MemberDTO>>, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireMemberAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out _))
            {
                return new NotFound();
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
