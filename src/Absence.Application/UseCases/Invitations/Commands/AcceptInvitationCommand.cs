using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Commands;

public class AcceptInvitationCommand(int id, bool accespted) : IRequest<OneOf<Success, NotFound, AccessDenied>>
{
    public int Id { get; } = id;
    public bool Accespted { get; } = accespted;
}