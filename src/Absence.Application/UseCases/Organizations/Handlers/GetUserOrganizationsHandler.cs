using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Application.UseCases.Organizations.Queries;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Handlers;

internal class GetUserOrganizationsHandler(
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetUserOrganizationsQuery, IEnumerable<OrganizationDTO>>
{
    public async Task<IEnumerable<OrganizationDTO>> Handle(GetUserOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizationUserEntities = await organizationUserRepository.GetAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId)
            ],
            cancellationToken
        );

        return mapper.Map<IEnumerable<OrganizationDTO>>(organizationUserEntities, opts => opts.Items["UserId"] = user.ShortId);
    }
}