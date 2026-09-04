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
        IUser user,
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

            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == holiday.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new NotFound();
            }
            if (!organizationUser.IsAdmin)
            {
                return new AccessDenied();
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
