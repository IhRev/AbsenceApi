using Absence.Application.Common.Results;
using Absence.Application.UseCases.Departments.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Queries;

public record GetDepartmentsQuery(int OrganizationId) : IRequest<OneOf<Success<IEnumerable<DepartmentDTO>>, AccessDenied>>;