using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Invitations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Commands;

public record InviteUserToOrganizationCommand(int OrganizationId, InviteUserToOrganizationDTO Invite)
    : IRequest<OneOf<Success, BadRequest>>, IRequirePermission
{
    public string Permission => PermissionNames.MANAGE_PERMISSIONS;
}