using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public class DeleteOrganizationCommand(int id) : IRequest<OneOf<Success, AccessDenied, NotFound>>
{
    public int Id { get; } = id;
}