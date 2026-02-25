using Absence.Application.UseCases.Departments.Commands;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

internal class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, OneOf<Success, NotFound>>
{
    public Task<OneOf<Success, NotFound>> Handle(
        DeleteDepartmentCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}