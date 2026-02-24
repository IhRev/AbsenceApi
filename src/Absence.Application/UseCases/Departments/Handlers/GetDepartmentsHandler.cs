using Absence.Application.Common.Results;
using Absence.Application.UseCases.Departments.DTOs;
using Absence.Application.UseCases.Departments.Queries;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

public class GetDepartmentsHandler : IRequestHandler<GetDepartmentsQuery, OneOf<Success<IEnumerable<DepartmentDTO>>, AccessDenied>>
{
    public Task<OneOf<Success<IEnumerable<DepartmentDTO>>, AccessDenied>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}