using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetUserAbsences
{
    public sealed class Query(DateTimeOffset startDate, DateTimeOffset endDate, int organizationId)
        : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, NotFound>>
    {
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceDTO>>, NotFound>>
    {
        public async Task<OneOf<Success<IEnumerable<AbsenceDTO>>, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireMemberAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out _))
            {
                return new NotFound();
            }

            var absences = await db.Absences
                .Where(_ => _.UserId == user.ShortId)
                .Where(_ => _.StartDate < request.EndDate && _.EndDate > request.StartDate)
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .Select(_ => new AbsenceDTO
                {
                    Id = _.Id,
                    Name = _.Name,
                    Type = _.AbsenceTypeId,
                    UserId = _.UserId,
                    StartDate = _.StartDate,
                    EndDate = _.EndDate
                })
                .ToListAsync(cancellationToken);
            return new Success<IEnumerable<AbsenceDTO>>(absences);
        }
    }
}
