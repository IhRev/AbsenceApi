using Absence.Application.UseCases.Invitations.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Invitations.Queries;

public record GetUserInvitationsQuery : IRequest<IEnumerable<InvitationDTO>>;