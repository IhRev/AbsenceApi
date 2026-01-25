using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class EditOrganizationHandler(
    IRepository<OrganizationEntity> organizationRepository,
    IUser user
) : IRequestHandler<EditOrganizationCommand, OneOf<Success, NotFound, BadRequest, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(EditOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.Organization.Id);
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
        organizationRepository.Update(organization);
        await organizationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}