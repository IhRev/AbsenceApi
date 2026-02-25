using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Departments.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Departments.Commands;

public record AddDepartmentCommand(int OrganizationId, CreateDepartmentDTO Department)
    : IRequest<int>, IRequirePermission
{
    public string Permission => Permissions.MANAGE_DEPARTMENTS;
}