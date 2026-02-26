using Absence.Application.UseCases.Departments.DTOs;
using Absence.Application.UseCases.Departments.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Departments.Handlers;

public class GetAllDepartmentsHandler(IRepository<DepartmentEntity> departmentRepository, IMapper mapper)
    : IRequestHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentDTO>>
{
    public async Task<IEnumerable<DepartmentDTO>> Handle(
        GetAllDepartmentsQuery request, 
        CancellationToken cancellationToken = default
    )
    {
        var departments = await departmentRepository.GetAsync(new DepartmentSpec(request.OrganizationId));
        return mapper.Map<IEnumerable<DepartmentDTO>>(departments);
    }
}