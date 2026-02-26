using Absence.Application.UseCases.Departments.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Departments.Handlers;

internal class AddDepartmentHandler(IRepository<DepartmentEntity> departmentRepository, IMapper mapper)
    : IRequestHandler<AddDepartmentCommand, int>
{
    public async Task<int> Handle(AddDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        var department = mapper.Map<DepartmentEntity>(request.Department);
        await departmentRepository.InsertAsync(department, cancellationToken);
        await departmentRepository.SaveAsync(cancellationToken);
        return department.Id;
    }
}