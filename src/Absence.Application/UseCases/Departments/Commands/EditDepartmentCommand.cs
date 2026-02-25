using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Departments.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Commands;

public record EditDepartmentCommand(int OrganizationId, EditDepartmentDTO Department)
    : IRequest<OneOf<Success, NotFound>>, IRequirePermission
{
    public string Permission => Permissions.MANAGE_DEPARTMENTS;
}