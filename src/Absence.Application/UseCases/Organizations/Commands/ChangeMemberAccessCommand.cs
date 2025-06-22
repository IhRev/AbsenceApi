using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public class ChangeMemberAccessCommand(int organizationId, int userId, bool isAdmin) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
{
    public int OrganizationId { get; } = organizationId;
    public int UserId { get; } = userId;
    public bool IsAdmin { get; } = isAdmin;
}