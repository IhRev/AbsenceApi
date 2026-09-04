using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using MediatR;

namespace Absence.Api.Features.Organizations;

public class CreateOrganizationDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}

public static class AddOrganization
{
    public sealed class Command(CreateOrganizationDTO organization) : IRequest<int>
    {
        public CreateOrganizationDTO Organization { get; } = organization;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var organization = new OrganizationEntity
            {
                Name = request.Organization.Name,
                OwnerId = user.ShortId
            };
            organization.OrganizationsUsers.Add(new OrganizationUserEntity()
            {
                IsAdmin = true,
                UserId = user.ShortId
            });
            db.Organizations.Add(organization);
            await db.SaveChangesAsync(cancellationToken);

            return organization.Id;
        }
    }
}
