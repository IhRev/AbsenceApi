using Absence.Application.Common.Results;
using Absence.Application.UseCases.Departments.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Departments.Commands;

public record AddDepartmentCommand(CreateDepartmentDTO Department) : IRequest<OneOf<Success<int>, AccessDenied>>;