using Absence.Application.Common.Results;
using Absence.Application.UseCases.Departments.Commands;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Handlers;

internal class AddDepartmentHandler : IRequestHandler<AddDepartmentCommand, OneOf<Success<int>, AccessDenied>>
{
    public Task<OneOf<Success<int>, AccessDenied>> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}