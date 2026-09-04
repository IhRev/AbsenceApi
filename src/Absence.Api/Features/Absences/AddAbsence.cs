using System.ComponentModel.DataAnnotations;
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

public class CreateAbsenceDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public int Type { get; set; }
    [Required]
    public DateTimeOffset StartDate { get; set; }
    [Required]
    public DateTimeOffset EndDate { get; set; }
    [Required]
    public int Organization { get; set; }
}

public static class AddAbsence
{
    public sealed class Command(CreateAbsenceDTO absence) : IRequest<OneOf<Success<int>, Success<string>, NotFound, BadRequest>>
    {
        public CreateAbsenceDTO Absence { get; } = absence;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IAbsenceHolidayOverlapChecker overlapChecker,
        IOrganizationAccess organizationAccess,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success<int>, Success<string>, NotFound, BadRequest>>
    {
        public async Task<OneOf<Success<int>, Success<string>, NotFound, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireMemberAsync(request.Absence.Organization, cancellationToken);
            if (!access.TryPickT0(out var organizationUser, out _))
            {
                return new NotFound();
            }

            var absenceType = await db.AbsenceTypes.FirstOrDefaultAsync(
                _ => _.Id == request.Absence.Type,
                cancellationToken);
            if (absenceType is null)
            {
                return new BadRequest($"No absence type with id {request.Absence.Type} found.");
            }

            if (request.Absence.StartDate > request.Absence.EndDate)
            {
                return new BadRequest("Start date must be before end date.");
            }

            if (await overlapChecker.AbsenceOverlapsHolidayAsync(
                request.Absence.Organization,
                request.Absence.StartDate,
                request.Absence.EndDate,
                cancellationToken))
            {
                return new BadRequest("Absence overlaps a holiday.");
            }

            if (organizationUser.IsAdmin)
            {
                var absence = new AbsenceEntity
                {
                    Name = request.Absence.Name,
                    AbsenceTypeId = request.Absence.Type,
                    OrganizationId = request.Absence.Organization,
                    StartDate = request.Absence.StartDate,
                    EndDate = request.Absence.EndDate,
                    UserId = user.ShortId
                };
                db.Absences.Add(absence);
                await db.SaveChangesAsync(cancellationToken);
                return new Success<int>(absence.Id);
            }

            var absenceEvent = new AbsenceEventEntity
            {
                Name = request.Absence.Name,
                AbsenceTypeId = request.Absence.Type,
                OrganizationId = request.Absence.Organization,
                StartDate = request.Absence.StartDate,
                EndDate = request.Absence.EndDate,
                UserId = user.ShortId,
                AbsenceEventType = AbsenceEventType.CREATE
            };
            db.AbsenceEvents.Add(absenceEvent);
            await db.SaveChangesAsync(cancellationToken);
            return new Success<string>("Absence create requested.");
        }
    }
}
