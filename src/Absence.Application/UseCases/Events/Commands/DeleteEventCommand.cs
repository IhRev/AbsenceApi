using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Constants;

namespace Absence.Application.UseCases.Events.Commands;

public record DeleteEventCommand(int OrganizationId, int Id) 
    : IRequest<OneOf<Success, NotFound>>, IRequirePermission
{
    public string Permission => PermissionNames.MANAGE_EVENTS;
}