using Absence.Application.UseCases.Departments.Commands;
using MediatR;

namespace Absence.Application.UseCases.Departments.Handlers;

internal class AddDepartmentHandler : IRequestHandler<AddDepartmentCommand, int>
{
    public Task<int> Handle(AddDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}