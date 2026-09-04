using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Repositories;
using AutoMapper;
using MediatR;

namespace Absence.Api.Features.Invitations;

public static class GetUserInvitations
{
    public sealed class Query : IRequest<IEnumerable<InvitationDTO>>;

    internal sealed class Handler(
        IUser user,
        IOrganizationUserInvitationsRepository organizationUserInvitationRepository,
        IMapper mapper
    ) : IRequestHandler<Query, IEnumerable<InvitationDTO>>
    {
        private readonly IUser _user = user;
        private readonly IOrganizationUserInvitationsRepository _organizationUserInvitationRepository = organizationUserInvitationRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<InvitationDTO>> Handle(Query request, CancellationToken cancellationToken)
        {
            var invitations = await _organizationUserInvitationRepository.GetAsync(
                [
                    q => q.Where(_ => _.Invited == _user.ShortId)
                ],
                cancellationToken
            );
            return _mapper.Map<IEnumerable<InvitationDTO>>(invitations);
        }
    }
}
