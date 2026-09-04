using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetUsersAbsences
{
    public sealed class Query(DateTimeOffset startDate, DateTimeOffset endDate, int organizationId, List<int> userIds)
        : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
    {
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
        public int OrganizationId { get; } = organizationId;
        public List<int> UserIds { get; } = userIds;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IMapper mapper,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>> Handle(Query request, CancellationToken cancellationToken)
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

            var absences = await db.Absences
                .Where(_ => request.UserIds.Contains(_.UserId))
                .Where(_ => _.StartDate < request.EndDate && _.EndDate > request.StartDate)
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .ToListAsync(cancellationToken);
            return new Success<IEnumerable<AbsenceDTO>>(mapper.Map<IEnumerable<AbsenceDTO>>(absences));
        }
    }
}
