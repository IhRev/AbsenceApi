using Absence.Application.Common.Results;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Absences.DTOs;

namespace Absence.Application.UseCases.Absences.Commands;

public record EditAbsenceCommand(EditAbsenceDTO Absence) : IRequest<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>;