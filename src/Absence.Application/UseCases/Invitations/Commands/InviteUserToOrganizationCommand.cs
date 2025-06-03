using Absence.Application.Common.Results;
using Absence.Application.UseCases.Invitations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Commands;

public class InviteUserToOrganizationCommand(InviteUserToOrganizationDTO invite) : IRequest<OneOf<Success, BadRequest, AccessDenied>>
{
    public InviteUserToOrganizationDTO Invite { get; } = invite;
}