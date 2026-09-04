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
    public sealed class Command(CreateHolidayDTO holiday) : IRequest<OneOf<Success<int>, BadRequest, AccessDenied>>
    {
        public CreateHolidayDTO Holiday { get; } = holiday;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user,
        IAbsenceHolidayOverlapChecker overlapChecker
    ) : IRequestHandler<Command, OneOf<Success<int>, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success<int>, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == request.Holiday.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"No organization with id {request.Holiday.OrganizationId} found.");
            }
            if (!organizationUser.IsAdmin)
            {
                return new AccessDenied();
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
