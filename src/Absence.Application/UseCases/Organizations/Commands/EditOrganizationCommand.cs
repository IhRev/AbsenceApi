using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public record EditOrganizationCommand(EditOrganizationDTO Organization) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>;