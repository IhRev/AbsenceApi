using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Absences.Commands;

public record AddAbsenceCommand(CreateAbsenceDTO Absence) : IRequest<OneOf<Success<int>, Success<string>, BadRequest>>;