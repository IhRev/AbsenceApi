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
        IUser user,
        IMapper mapper
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
                }

                switch (absenceEvent.AbsenceEventType)
                {
                    case AbsenceEventType.CREATE:
                        await AddAbsence(absenceEvent, cancellationToken);
                        break;
                    case AbsenceEventType.UPDATE:
                        await UpdateAbsence(absenceEvent, cancellationToken);
                        break;
                    case AbsenceEventType.DELETE:
                        await DeleteAbsence(absenceEvent, cancellationToken);
                        break;
                    default:
                        throw new ArgumentException($"Incorrect event type {absenceEvent.AbsenceEventType}");
                }
                await db.SaveChangesAsync(cancellationToken);
            }

            db.AbsenceEvents.Remove(absenceEvent);
            await db.SaveChangesAsync(cancellationToken);
            return new Success();
        }

        private Task AddAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            var absence = mapper.Map<AbsenceEntity>(absenceEvent);
            db.Absences.Add(absence);
            return Task.CompletedTask;
        }

        private async Task UpdateAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            if (absenceEvent.AbsenceId is not int absenceId)
            {
                return;
            }

            var absence = await db.Absences.FirstOrDefaultAsync(_ => _.Id == absenceId, cancellationToken);
            if (absence is null)
            {
                return;
            }

            mapper.Map(absenceEvent, absence);
        }

        private async Task DeleteAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            if (absenceEvent.AbsenceId is not int absenceId)
            {
                return;
            }

            var absence = await db.Absences.FirstOrDefaultAsync(_ => _.Id == absenceId, cancellationToken);
            if (absence is null)
            {
                return;
            }

            db.Absences.Remove(absence);
        }
    }
}
