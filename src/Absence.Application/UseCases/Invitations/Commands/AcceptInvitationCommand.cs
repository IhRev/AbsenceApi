using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Commands;

public record AcceptInvitationCommand(int Id, bool Accespted) : IRequest<OneOf<Success, NotFound, AccessDenied>>;