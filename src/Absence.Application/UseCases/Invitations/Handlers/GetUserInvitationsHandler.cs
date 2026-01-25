using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.DTOs;
using Absence.Application.UseCases.Invitations.Queries;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Invitations.Handlers;

public class GetUserInvitationsHandler(
    IUser user,
    IOrganizationUserInvitationsRepository organizationUserInvitationRepository,
    IMapper mapper
) : IRequestHandler<GetUserInvitationsQuery, IEnumerable<InvitationDTO>>
{
    public async Task<IEnumerable<InvitationDTO>> Handle(GetUserInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitations = await organizationUserInvitationRepository.GetAsync(
            [
                q => q.Where(_ => _.Invited == user.ShortId)
            ], 
            cancellationToken
        );
        return mapper.Map<IEnumerable<InvitationDTO>>(invitations);
    }
}