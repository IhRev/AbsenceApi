using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public record ChangeMemberAccessCommand(int OrganizationId, int UserId, bool IsAdmin) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>;