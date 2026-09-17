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
    public sealed class Command(int id, DeleteOrganizationRequest request) : IRequest<OneOf<Success, NotFound, AccessDenied, Unauthorized>>
    {
        public int Id { get; } = id;
        public DeleteOrganizationRequest Request { get; } = request;
    }

    internal sealed class Handler(
        IUser user,
        AbsenceContext db,
        IOrganizationAccess organizationAccess,
        IUserService userService
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, Unauthorized>>
    {
        public async Task<OneOf<Success, NotFound, AccessDenied, Unauthorized>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireOwnerAsync(request.Id, cancellationToken);
            if (!access.TryPickT0(out var organization, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, AccessDenied, Unauthorized>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            var identityUser = await userService.FindByIdAsync(user.Id);
            if (identityUser is null)
            {
                return new Unauthorized();
            }
            if (!await userService.CheckPasswordAsync(identityUser, request.Request.Password))
            {
                return new AccessDenied();
            }

            db.Organizations.Remove(organization);
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}