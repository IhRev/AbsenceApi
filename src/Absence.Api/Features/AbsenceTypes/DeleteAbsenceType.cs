using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.AbsenceTypes;

public static class DeleteAbsenceType
{
    public sealed class Command(int organizationId, int typeId) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
        public int TypeId { get; } = typeId;
    }

    internal sealed class Handler(AbsenceContext db, IOrganizationAccess organizationAccess)
        : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireAdminAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, BadRequest, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            var type = await db.AbsenceTypes.FirstOrDefaultAsync(
                _ => _.Id == request.TypeId && _.OrganizationId == request.OrganizationId,
                cancellationToken);
            if (type is null)
            {
                return new NotFound();
            }

            // Absences reference this type with OnDelete(Restrict). Pending events store the type id
            // with no foreign key, so both have to be refused here rather than at the database.
            if (await db.Absences.AnyAsync(_ => _.AbsenceTypeId == type.Id, cancellationToken))
            {
                return new BadRequest("This absence type is still used by an absence.");
            }

            if (await db.AbsenceEvents.AnyAsync(_ => _.AbsenceTypeId == type.Id, cancellationToken))
            {
                return new BadRequest("This absence type is still referenced by a pending request.");
            }

            db.AbsenceTypes.Remove(type);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
