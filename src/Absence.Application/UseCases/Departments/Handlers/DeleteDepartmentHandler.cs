using Absence.Application.UseCases.Departments.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

internal class DeleteDepartmentHandler(IRepository<DepartmentEntity> departmentRepository) 
    : IRequestHandler<DeleteDepartmentCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        DeleteDepartmentCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var department = await departmentRepository.GetByIdAsync(request.Id);
        if (department is null || department.IsDeleted)
        {
            return new NotFound();
        }

        departmentRepository.Delete(department);
        await departmentRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}