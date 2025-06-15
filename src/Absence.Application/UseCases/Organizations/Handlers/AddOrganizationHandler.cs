using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
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
    private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
    private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<int> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = _mapper.Map<OrganizationEntity>(request.Organization);
        organization.OwnerId = _user.ShortId;
        await _organizationRepository.InsertAsync(organization, cancellationToken);
        await _organizationRepository.SaveAsync(cancellationToken);

        var organizationUser = new OrganizationUserEntity()
        {
            IsAdmin = true,
            OrganizationId = organization.Id,
            UserId = _user.ShortId
        };
        await _organizationUserRepository.InsertAsync(organizationUser);
        await _organizationUserRepository.SaveAsync();

        return organization.Id;
    }
}