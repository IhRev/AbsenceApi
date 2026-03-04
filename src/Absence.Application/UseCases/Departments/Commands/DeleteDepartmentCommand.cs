using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Commands;

public record DeleteDepartmentCommand(int OrganizationId, int Id) 
    : IRequest<OneOf<Success, NotFound>>, IRequirePermission
{
    public string Permission => PermissionNames.MANAGE_DEPARTMENTS;
}