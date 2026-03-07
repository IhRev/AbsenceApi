using Absence.Application.UseCases.Departments.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

public class EditDepartmentHandler(IRepository<DepartmentEntity> departmentRepository, IMapper mapper)
    : IRequestHandler<EditDepartmentCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        EditDepartmentCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var department = await departmentRepository.GetByIdAsync(request.Department.Id);
        if (department is null)
        {
            return new NotFound();
        }

        mapper.Map(request.Department, department);
        departmentRepository.Update(department);
        await departmentRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}