using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.DTOs;
using Absence.Application.UseCases.Invitations.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Invitations.Handlers;

public class GetUserInvitationsHandler(
    IRepository<OrganizationUserInvitationEntity> invitationRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<GetUserInvitationsQuery, IEnumerable<InvitationDTO>>
{
    public async Task<IEnumerable<InvitationDTO>> Handle(
        GetUserInvitationsQuery request, 
        CancellationToken cancellationToken = default
    )
    {
        var invitations = await invitationRepository.GetAsync(
            new InvitationSpec(user.ShortId),
            cancellationToken
        );
        return mapper.Map<IEnumerable<InvitationDTO>>(invitations);
    }
}