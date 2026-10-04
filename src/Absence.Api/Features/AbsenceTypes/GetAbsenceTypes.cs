using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.AbsenceTypes;

public class AbsenceTypeDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}

public static class GetAbsenceTypes
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<AbsenceTypeDTO>>, NotFound>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(AbsenceContext db, IOrganizationAccess organizationAccess)
        : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceTypeDTO>>, NotFound>>
    {
        public async Task<OneOf<Success<IEnumerable<AbsenceTypeDTO>>, NotFound>> Handle(Query request, CancellationToken cancellationToken = default)
        {
            var access = await organizationAccess.RequireMemberAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out _))
            {
                return new NotFound();
            }

            var types = await db.AbsenceTypes
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .Select(_ => new AbsenceTypeDTO
                {
                    Id = _.Id,
                    Name = _.Name
                })
                .ToListAsync(cancellationToken);

            return new Success<IEnumerable<AbsenceTypeDTO>>(types);
        }
    }
}
