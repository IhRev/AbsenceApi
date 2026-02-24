using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Commands;

public record DeleteDepartmentCommand(int Id) : IRequest<OneOf<Success, NotFound, AccessDenied>>;