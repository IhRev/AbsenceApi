using Absence.Application.Common.Results;
using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Organizations.Commands;

public class DeleteOrganizationCommand(int id, DTOs.DeleteOrganizationRequest request) : IRequest<OneOf<Success, AccessDenied, NotFound>>
{
    public int Id { get; } = id;
    public DeleteOrganizationRequest Request { get; } = request;
}