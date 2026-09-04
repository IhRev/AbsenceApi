using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetUserAbsences
{
    public sealed class Query(DateTimeOffset startDate, DateTimeOffset endDate, int organizationId)
        : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest>>
    {
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest>>
    {
        public async Task<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest>> Handle(Query request, CancellationToken cancellationToken)
        {
            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == request.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"No organization with id {request.OrganizationId} found.");
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
