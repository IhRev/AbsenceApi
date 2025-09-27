using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public class DeleteMemberCommand(int organizationId, int memberId) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
{
    public int OrganizationId { get; } = organizationId;
    public int MemberId { get; } = memberId;
}