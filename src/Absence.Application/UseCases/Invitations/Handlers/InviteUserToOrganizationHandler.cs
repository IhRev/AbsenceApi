using Absence.Application.Common.Results;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Interfaces;
using Absence.Domain.Entities;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Invitations.Handlers;

public class InviteUserToOrganizationHandler(
    IRepository<OrganizationUserInvitationEntity> organizationUserInvitationRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IRepository<OrganizationEntity> organizationRepository,
    IRepository<OrganizationUserInvitationEntity> invitationRepository,
    IUserService userService,
    IUser user
) : IRequestHandler<InviteUserToOrganizationCommand, OneOf<Success, BadRequest, AccessDenied>>
{
    private readonly IRepository<OrganizationUserInvitationEntity> _organizationUserInvitationRepository = organizationUserInvitationRepository;
    private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
    private readonly IRepository<OrganizationEntity> _organizationRepository = organizationRepository;
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, BadRequest, AccessDenied>> Handle(InviteUserToOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = _organizationRepository.GetByIdAsync(request.Invite.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return new BadRequest($"Organization with id {request.Invite.OrganizationId} doesn't exist.");
        }

        var inviterOrganization = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ =>
                    _.OrganizationId == request.Invite.OrganizationId &&
                    _.UserId == _user.ShortId
                )
            ],
            cancellationToken
        );
        if (inviterOrganization is null || !inviterOrganization.IsAdmin)
        {
            return new AccessDenied();
        }

        var invitedUser = await _userService.FindByEmailAsync(request.Invite.UserEmail);
        if (invitedUser is null)
        {
            return new BadRequest($"User with email {request.Invite.UserEmail} doesn't exist.");
        }

        var invitedUserOrganization = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ =>
                    _.OrganizationId == request.Invite.OrganizationId &&
                    _.UserId == invitedUser.ShortId
                )
            ],
            cancellationToken
        );
        if (invitedUserOrganization is not null)
        {
            return new BadRequest($"Invited user already belongs to organization.");
        }

        var invitation = await invitationRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => 
                    _.OrganizationId == request.Invite.OrganizationId && 
                    _.UserId == invitedUser.ShortId
                )
            ],
            cancellationToken
        );
        if (invitation is not null)
        {
            return new BadRequest("Invitation already sent.");
        }

        await _organizationUserInvitationRepository.InsertAsync(
            new OrganizationUserInvitationEntity()
            {
                UserId = invitedUser.ShortId,
                OrganizationId = request.Invite.OrganizationId
            }, 
            cancellationToken
        );
        await _organizationUserInvitationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}