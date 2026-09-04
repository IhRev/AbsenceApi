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
        IOrganizationAccess organizationAccess
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var holiday = await db.Holidays.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken);
            if (holiday is null)
            {
                return new NotFound();
            }

            var access = await organizationAccess.RequireAdminAsync(holiday.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            db.Holidays.Remove(holiday);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
