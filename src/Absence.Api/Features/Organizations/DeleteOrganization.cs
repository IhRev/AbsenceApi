using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public class DeleteOrganizationRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}

public static class DeleteOrganization
{
    public sealed class Command(int id, DeleteOrganizationRequest request) : IRequest<OneOf<Success, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
        public DeleteOrganizationRequest Request { get; } = request;
    }

    internal sealed class Handler(
        IUser user,
        AbsenceContext db,
        IUserService userService
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identityUser = await userService.FindByIdAsync(user.Id);
            if (!await userService.CheckPasswordAsync(identityUser!, request.Request.Password))
            {
                return new AccessDenied();
            }

            var organization = await db.Organizations.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken);
            if (organization is null)
            {
                return new NotFound();
            }
            if (organization.OwnerId != user.ShortId)
            {
                return new AccessDenied();
            }

            db.Organizations.Remove(organization);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
