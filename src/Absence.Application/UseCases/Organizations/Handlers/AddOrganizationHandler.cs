using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Handlers;

internal class AddOrganizationHandler(
    IRepository<OrganizationEntity> organizationRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddOrganizationCommand, int>
{
    public async Task<int> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = mapper.Map<OrganizationEntity>(request.Organization);
        organization.OwnerId = user.ShortId;
        await organizationRepository.InsertAsync(organization, cancellationToken);
        await organizationRepository.SaveAsync(cancellationToken);

        var organizationUser = new OrganizationUserEntity()
        {
            IsAdmin = true,
            OrganizationId = organization.Id,
            UserId = user.ShortId
        };
        await organizationUserRepository.InsertAsync(organizationUser);
        await organizationUserRepository.SaveAsync();

        return organization.Id;
    }
}