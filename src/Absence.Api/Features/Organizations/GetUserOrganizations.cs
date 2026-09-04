using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Repositories;
using AutoMapper;
using MediatR;

namespace Absence.Api.Features.Organizations;

public static class GetUserOrganizations
{
    public sealed class Query : IRequest<IEnumerable<OrganizationDTO>>;

    internal sealed class Handler(
        IOrganizationUsersRepository organizationUserRepository,
        IMapper mapper,
        IUser user
    ) : IRequestHandler<Query, IEnumerable<OrganizationDTO>>
    {
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;

        public async Task<IEnumerable<OrganizationDTO>> Handle(Query request, CancellationToken cancellationToken)
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
}
