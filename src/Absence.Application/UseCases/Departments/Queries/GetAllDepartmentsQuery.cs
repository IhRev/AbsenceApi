using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Departments.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Departments.Queries;

public record GetAllDepartmentsQuery(int OrganizationId) 
    : IRequest<IEnumerable<DepartmentDTO>>, IRequirePermission
{
    public string Permission => Permissions.VIEW_DEPARTMENTS;
}