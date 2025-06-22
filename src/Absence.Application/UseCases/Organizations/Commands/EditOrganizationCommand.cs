using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public class EditOrganizationCommand(EditOrganizationDTO organization) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
{
    public EditOrganizationDTO Organization { get; } = organization;
}