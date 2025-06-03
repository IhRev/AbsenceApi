using Absence.Application.UseCases.Invitations.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Invitations.Queries;

public class GetUserInvitationsQuery : IRequest<IEnumerable<InvitationDTO>>;