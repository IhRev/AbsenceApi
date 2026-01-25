using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public record DeleteOrganizationCommand(int Id, DeleteOrganizationRequest Request) : IRequest<OneOf<Success, AccessDenied, NotFound>>;