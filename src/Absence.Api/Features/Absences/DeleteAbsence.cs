using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Common;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class DeleteAbsence
{
    public sealed class Command(int id) : IRequest<OneOf<Success<string>, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success<string>, NotFound, AccessDenied>>
    {
        public async Task<OneOf<Success<string>, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var absence = await db.Absences.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken);
            if (absence is null)
            {
                return new NotFound();
            }
            if (absence.UserId != user.ShortId)
            {
                return new AccessDenied();
            }

            var access = await organizationAccess.RequireMemberAsync(absence.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out var organizationUser, out _))
            {
                return new NotFound();
            }
            if (organizationUser.IsAdmin)
            {
                db.Absences.Remove(absence);
                await db.SaveChangesAsync(cancellationToken);
                return new Success<string>("Absence deleted.");
            }

            db.AbsenceEvents.Add(new AbsenceEventEntity
            {
                Name = absence.Name,
                StartDate = absence.StartDate,
                EndDate = absence.EndDate,
                AbsenceTypeId = absence.AbsenceTypeId,
                UserId = absence.UserId,
                OrganizationId = absence.OrganizationId,
                AbsenceId = absence.Id,
                AbsenceEventType = AbsenceEventType.DELETE
            });
            await db.SaveChangesAsync(cancellationToken);
            return new Success<string>("Absence delete requested.");
        }
    }
}
