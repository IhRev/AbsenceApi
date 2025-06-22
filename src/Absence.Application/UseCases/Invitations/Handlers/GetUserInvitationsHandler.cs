using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.DTOs;
using Absence.Application.UseCases.Invitations.Queries;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Invitations.Handlers;

public class GetUserInvitationsHandler(
    IUser user,
    IOrganizationUserInvitationsRepository organizationUserInvitationRepository,
    IMapper mapper
) : IRequestHandler<GetUserInvitationsQuery, IEnumerable<InvitationDTO>>
{
    private readonly IUser _user = user;
    private readonly IOrganizationUserInvitationsRepository _organizationUserInvitationRepository = organizationUserInvitationRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<InvitationDTO>> Handle(GetUserInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitations = await _organizationUserInvitationRepository.GetAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId)
            ], 
            cancellationToken
        );
        return _mapper.Map<IEnumerable<InvitationDTO>>(invitations);
    }
}