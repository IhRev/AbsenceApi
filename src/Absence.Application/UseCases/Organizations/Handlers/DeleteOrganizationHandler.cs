using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.Identity;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Handlers;

public class DeleteOrganizationHandler(
    IUser user,
    IRepository<OrganizationEntity> organizationRepository,
    IUserService userService
) : IRequestHandler<DeleteOrganizationCommand, OneOf<Success, AccessDenied, NotFound>>
{
    public async Task<OneOf<Success, AccessDenied, NotFound>> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        if (!await userService.CheckPasswordAsync(userEntity!, request.Request.Password))
        {
            return new AccessDenied();
        }

        var organization = await organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organization is null)
        {
            return new NotFound();
        }
        if (organization.OwnerId != user.ShortId)
        {
            return new AccessDenied();
        }

        organizationRepository.Delete(organization);
        await organizationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}