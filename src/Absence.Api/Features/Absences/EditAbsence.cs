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

public class EditAbsenceDTO
{
    [Required]
    public required int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public required int Type { get; set; }
    [Required]
    public required DateTimeOffset StartDate { get; set; }
    [Required]
    public required DateTimeOffset EndDate { get; set; }
}

public static class EditAbsence
{
    public sealed class Command(EditAbsenceDTO absence) : IRequest<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>
    {
        public EditAbsenceDTO Absence { get; } = absence;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user,
        IOrganizationAccess organizationAccess,
        IAbsenceHolidayOverlapChecker overlapChecker
    ) : IRequestHandler<Command, OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var absence = await db.Absences.FirstOrDefaultAsync(_ => _.Id == request.Absence.Id, cancellationToken);
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

            if (request.Absence.StartDate > request.Absence.EndDate)
            {
                return new BadRequest("Start date must be before end date.");
            }

            if (await overlapChecker.AbsenceOverlapsHolidayAsync(
                absence.OrganizationId,
                request.Absence.StartDate,
                request.Absence.EndDate,
                cancellationToken))
            {
                return new BadRequest("Absence overlaps a holiday.");
            }

            if (absence.AbsenceTypeId != request.Absence.Type)
            {
                var typeExists = await db.AbsenceTypes.AnyAsync(_ => _.Id == request.Absence.Type, cancellationToken);
                if (!typeExists)
                {
                    return new BadRequest($"Type with id {request.Absence.Type} doesn't exist");
                }
            }

            if (organizationUser.IsAdmin)
            {
                absence.Name = request.Absence.Name;
                absence.AbsenceTypeId = request.Absence.Type;
                absence.StartDate = request.Absence.StartDate;
                absence.EndDate = request.Absence.EndDate;
                await db.SaveChangesAsync(cancellationToken);

                return new Success<string>("Absence updated.");
            }

            var absenceEvent = new AbsenceEventEntity
            {
                Name = request.Absence.Name,
                AbsenceId = request.Absence.Id,
                AbsenceTypeId = request.Absence.Type,
                StartDate = request.Absence.StartDate,
                EndDate = request.Absence.EndDate,
                AbsenceEventType = AbsenceEventType.UPDATE,
                OrganizationId = absence.OrganizationId,
                UserId = absence.UserId
            };
            db.AbsenceEvents.Add(absenceEvent);
            await db.SaveChangesAsync(cancellationToken);
            return new Success<string>("Absence update requested.");
        }
    }
}
