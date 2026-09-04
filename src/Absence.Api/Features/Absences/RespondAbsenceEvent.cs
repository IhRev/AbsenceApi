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

public static class RespondAbsenceEvent
{
    public sealed class Command(int id, bool accepted) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public int Id { get; } = id;
        public bool Accepted { get; } = accepted;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IAbsenceHolidayOverlapChecker overlapChecker,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var absenceEvent = await db.AbsenceEvents.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken);
            if (absenceEvent is null)
            {
                return new NotFound();
            }

            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == absenceEvent.OrganizationId,
                cancellationToken);
            if (organizationUser is null || !organizationUser.IsAdmin)
            {
                return new AccessDenied();
            }

            if (request.Accepted)
            {
                if (absenceEvent.AbsenceEventType is AbsenceEventType.CREATE or AbsenceEventType.UPDATE)
                {
                    if (absenceEvent.StartDate > absenceEvent.EndDate)
                    {
                        return new BadRequest("Start date must be before end date.");
                    }

                    if (await overlapChecker.AbsenceOverlapsHolidayAsync(
                        absenceEvent.OrganizationId,
                        absenceEvent.StartDate,
                        absenceEvent.EndDate,
                        cancellationToken))
                    {
                        return new BadRequest("Absence overlaps a holiday.");
                    }

                    var typeExists = await db.AbsenceTypes.AnyAsync(
                        _ => _.Id == absenceEvent.AbsenceTypeId,
                        cancellationToken);
                    if (!typeExists)
                    {
                        return new BadRequest($"Type with id {absenceEvent.AbsenceTypeId} doesn't exist");
                    }
                }

                switch (absenceEvent.AbsenceEventType)
                {
                    case AbsenceEventType.CREATE:
                        AddAbsence(absenceEvent);
                        break;
                    case AbsenceEventType.UPDATE:
                        if (!await UpdateAbsence(absenceEvent, cancellationToken))
                        {
                            return new BadRequest("The absence this event refers to no longer exists.");
                        }
                        break;
                    case AbsenceEventType.DELETE:
                        if (!await DeleteAbsence(absenceEvent, cancellationToken))
                        {
                            return new BadRequest("The absence this event refers to no longer exists.");
                        }
                        break;
                    default:
                        return new BadRequest($"Unsupported event type {absenceEvent.AbsenceEventType}.");
                }
            }

            db.AbsenceEvents.Remove(absenceEvent);
            await db.SaveChangesAsync(cancellationToken);
            return new Success();
        }

        private void AddAbsence(AbsenceEventEntity absenceEvent) =>
            db.Absences.Add(new AbsenceEntity
            {
                Name = absenceEvent.Name,
                StartDate = absenceEvent.StartDate,
                EndDate = absenceEvent.EndDate,
                AbsenceTypeId = absenceEvent.AbsenceTypeId,
                UserId = absenceEvent.UserId,
                OrganizationId = absenceEvent.OrganizationId
            });

        private async Task<bool> UpdateAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            if (absenceEvent.AbsenceId is not int absenceId)
            {
                return false;
            }

            var absence = await db.Absences.FirstOrDefaultAsync(_ => _.Id == absenceId, cancellationToken);
            if (absence is null)
            {
                return false;
            }

            absence.Name = absenceEvent.Name;
            absence.StartDate = absenceEvent.StartDate;
            absence.EndDate = absenceEvent.EndDate;
            absence.AbsenceTypeId = absenceEvent.AbsenceTypeId;
            absence.UserId = absenceEvent.UserId;
            absence.OrganizationId = absenceEvent.OrganizationId;
            return true;
        }

        private async Task<bool> DeleteAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            if (absenceEvent.AbsenceId is not int absenceId)
            {
                return false;
            }

            var absence = await db.Absences.FirstOrDefaultAsync(_ => _.Id == absenceId, cancellationToken);
            if (absence is null)
            {
                return false;
            }

            db.Absences.Remove(absence);
            return true;
        }
    }
}
