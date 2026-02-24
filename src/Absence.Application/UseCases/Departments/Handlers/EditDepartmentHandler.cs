using Absence.Application.Common.Results;
using Absence.Application.UseCases.Departments.Commands;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

public class EditDepartmentHandler : IRequestHandler<EditDepartmentCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public Task<OneOf<Success, NotFound, AccessDenied>> Handle(EditDepartmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}