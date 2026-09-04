using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public static class DeleteHoliday
{
    public sealed class Command(int id) : IRequest<OneOf<Success, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var holiday = await db.Holidays.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken);
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

            db.Holidays.Remove(holiday);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
