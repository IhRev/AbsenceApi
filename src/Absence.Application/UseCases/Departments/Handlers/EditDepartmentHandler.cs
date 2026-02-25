using Absence.Application.UseCases.Departments.Commands;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

public class EditDepartmentHandler : IRequestHandler<EditDepartmentCommand, OneOf<Success, NotFound>>
{
    public Task<OneOf<Success, NotFound>> Handle(
        EditDepartmentCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}