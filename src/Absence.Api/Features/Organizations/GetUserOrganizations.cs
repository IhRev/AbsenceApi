using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Absence.Api.Features.Organizations;

public static class GetUserOrganizations
{
    public sealed class Query : IRequest<IEnumerable<OrganizationDTO>>;

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Query, IEnumerable<OrganizationDTO>>
    {
        public async Task<IEnumerable<OrganizationDTO>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.OrganizationUsers
                .Where(_ => _.UserId == user.ShortId)
                .Select(_ => new OrganizationDTO
                {
                    Id = _.OrganizationId,
                    Name = _.Organization.Name,
                    IsAdmin = _.IsAdmin,
                    IsOwner = user.ShortId == _.Organization.OwnerId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
