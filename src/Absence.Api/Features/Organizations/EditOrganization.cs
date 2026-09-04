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
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organization = await db.Organizations.FirstOrDefaultAsync(_ => _.Id == request.Organization.Id, cancellationToken);
            if (organization is null)
            {
                return new NotFound();
            }
            if (organization.OwnerId != user.ShortId)
            {
                return new AccessDenied();
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
