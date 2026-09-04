using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetAbsenceEvents
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user,
        IMapper mapper
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>> Handle(Query request, CancellationToken cancellationToken)
        {
            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == request.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"No organization with id {request.OrganizationId} found.");
            }
            if (!organizationUser.IsAdmin)
            {
                return new AccessDenied();
            }

            var events = await db.AbsenceEvents
                .Include(_ => _.User)
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .ToListAsync(cancellationToken);
            return new Success<IEnumerable<AbsenceEventDTO>>(mapper.Map<IEnumerable<AbsenceEventDTO>>(events));
        }
    }
}
