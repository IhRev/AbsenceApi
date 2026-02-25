using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Constants;

namespace Absence.Application.UseCases.Events.Commands;

public record EditEventCommand(int OrganizationId, EditEventDTO Event)
    : IRequest<OneOf<Success, NotFound>>, IRequirePermission
{
    public string Permission => Permissions.MANAGE_EVENTS;
}