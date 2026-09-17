using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Organizations;

public class EditOrganizationDTO
{
    [Required]
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}

public static class EditOrganization
{
    public sealed class Command(EditOrganizationDTO organization) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public EditOrganizationDTO Organization { get; } = organization;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess
    ) : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireOwnerAsync(request.Organization.Id, cancellationToken);
            if (!access.TryPickT0(out var organization, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, BadRequest, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
            }

            if (organization.Name == request.Organization.Name)
            {
                return new BadRequest("New name should be different to old.");
            }

            organization.Name = request.Organization.Name;
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
