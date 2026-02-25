using Absence.Application.UseCases.Departments.DTOs;
using Absence.Application.UseCases.Departments.Queries;
using MediatR;

namespace Absence.Application.UseCases.Departments.Handlers;

public class GetDepartmentsHandler : IRequestHandler<GetDepartmentsQuery, IEnumerable<DepartmentDTO>>
{
    public Task<IEnumerable<DepartmentDTO>> Handle(
        GetDepartmentsQuery request, 
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}