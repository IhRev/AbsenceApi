using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetAbsenceEvents
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<AbsenceEventDTO>>, NotFound, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceEventDTO>>, NotFound, AccessDenied>>
    {
        public async Task<OneOf<Success<IEnumerable<AbsenceEventDTO>>, NotFound, AccessDenied>> Handle(Query request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireAdminAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success<IEnumerable<AbsenceEventDTO>>, NotFound, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            var events = await db.AbsenceEvents
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .Select(_ => new AbsenceEventDTO
                {
                    Id = _.Id,
                    Name = _.Name,
                    StartDate = _.StartDate,
                    EndDate = _.EndDate,
                    AbsenceType = _.AbsenceTypeId,
                    User = _.User.Email!,
                    AbsenceEventType = (int)_.AbsenceEventType
                })
                .ToListAsync(cancellationToken);
            return new Success<IEnumerable<AbsenceEventDTO>>(events);
        }
    }
}
