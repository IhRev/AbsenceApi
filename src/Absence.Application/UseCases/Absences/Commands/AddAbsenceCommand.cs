using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Absences.Commands;

public class AddAbsenceCommand(CreateAbsenceDTO absence) : IRequest<OneOf<Success<int>, Success<string>, BadRequest>>
{
    public CreateAbsenceDTO Absence { get; } = absence;
}