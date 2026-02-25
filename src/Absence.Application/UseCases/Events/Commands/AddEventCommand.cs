using MediatR;
using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Constants;

namespace Absence.Application.UseCases.Events.Commands;

public record AddEventCommand(int OrganizationId, CreateEventDTO Event) 
    : IRequest<int>, IRequirePermission
{
    public string Permission => Permissions.MANAGE_EVENTS;
}