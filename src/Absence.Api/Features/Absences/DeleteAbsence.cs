using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Common;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using AutoMapper;
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
        IUser user,
        IMapper mapper
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

            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == absence.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new AccessDenied();
            }
            if (organizationUser.IsAdmin)
            {
                db.Absences.Remove(absence);
                await db.SaveChangesAsync(cancellationToken);
                return new Success<string>("Absence deleted.");
            }

            var absenceEvent = mapper.Map<AbsenceEventEntity>(absence);
            absenceEvent.AbsenceEventType = AbsenceEventType.DELETE;
            db.AbsenceEvents.Add(absenceEvent);
            await db.SaveChangesAsync(cancellationToken);
            return new Success<string>("Absence delete requested.");
        }
    }
}
