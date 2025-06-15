using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Application.UseCases.Organizations.Queries;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Handlers;

internal class GetUserOrganizationsHandler(
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetUserOrganizationsQuery, IEnumerable<OrganizationDTO>>
{
    private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<IEnumerable<OrganizationDTO>> Handle(GetUserOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizationUserEntities = await _organizationUserRepository.GetAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId)
            ],
            cancellationToken
        );

        return _mapper.Map<IEnumerable<OrganizationDTO>>(organizationUserEntities, opts => opts.Items["UserId"] = _user.ShortId);
    }
} 