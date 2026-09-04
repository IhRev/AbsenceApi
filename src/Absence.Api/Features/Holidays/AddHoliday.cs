using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public class CreateHolidayDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required()]
    public required DateTimeOffset Date { get; set; }
    [Required()]
    public required int OrganizationId { get; set; }
}

public static class AddHoliday
{
    public sealed class Command(CreateHolidayDTO holiday) : IRequest<OneOf<Success<int>, NotFound, BadRequest, AccessDenied>>
    {
        public CreateHolidayDTO Holiday { get; } = holiday;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess,
        IAbsenceHolidayOverlapChecker overlapChecker
    ) : IRequestHandler<Command, OneOf<Success<int>, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success<int>, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireAdminAsync(request.Holiday.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success<int>, NotFound, BadRequest, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            if (await overlapChecker.HolidayOverlapsAbsenceAsync(
                request.Holiday.OrganizationId,
                request.Holiday.Date,
                cancellationToken))
            {
                return new BadRequest("Holiday overlaps an existing absence.");
            }

            var holiday = new HolidayEntity
            {
                Name = request.Holiday.Name,
                Date = request.Holiday.Date,
                OrganizationId = request.Holiday.OrganizationId
            };
            db.Holidays.Add(holiday);
            await db.SaveChangesAsync(cancellationToken);
            return new Success<int>(holiday.Id);
        }
    }
}
