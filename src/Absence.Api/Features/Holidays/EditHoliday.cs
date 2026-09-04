using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public class EditHolidayDTO
{
    [Required()]
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required()]
    public required DateTimeOffset Date { get; set; }
}

public static class EditHoliday
{
    public sealed class Command(EditHolidayDTO holiday) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public EditHolidayDTO Holiday { get; } = holiday;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess,
        IAbsenceHolidayOverlapChecker overlapChecker
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var holiday = await db.Holidays.FirstOrDefaultAsync(_ => _.Id == request.Holiday.Id, cancellationToken);
            if (holiday is null)
            {
                return new NotFound();
            }

            var access = await organizationAccess.RequireAdminAsync(holiday.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, AccessDenied, BadRequest>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            if (await overlapChecker.HolidayOverlapsAbsenceAsync(
                holiday.OrganizationId,
                request.Holiday.Date,
                cancellationToken))
            {
                return new BadRequest("Holiday overlaps an existing absence.");
            }

            holiday.Name = request.Holiday.Name;
            holiday.Date = request.Holiday.Date;
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
