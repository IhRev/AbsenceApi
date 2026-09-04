using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using MediatR;
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
        IRepository<OrganizationEntity> organizationRepository,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
        private readonly IUser _user = user;

        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organization = await _organizationRepository.GetByIdAsync(request.Organization.Id);
            if (organization is null)
            {
                return new NotFound();
            }
            if (organization.OwnerId != _user.ShortId)
            {
                return new AccessDenied();
            }

            if (organization.Name == request.Organization.Name)
            {
                return new BadRequest("New name should be different to old.");
            }

            organization.Name = request.Organization.Name;
            _organizationRepository.Update(organization);
            await _organizationRepository.SaveAsync(cancellationToken);

            return new Success();
        }
    }
}
