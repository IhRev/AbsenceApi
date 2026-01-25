using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public record DeleteMemberCommand(int OrganizationId, int MemberId) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>;