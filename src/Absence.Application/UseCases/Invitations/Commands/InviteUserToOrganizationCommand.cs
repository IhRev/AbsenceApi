using Absence.Application.Common.Results;
using Absence.Application.UseCases.Invitations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Commands;

public record InviteUserToOrganizationCommand(InviteUserToOrganizationDTO Invite) : IRequest<OneOf<Success, BadRequest, AccessDenied>>;