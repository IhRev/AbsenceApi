using Absence.Application.UseCases.Absences.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Commands;

public class AddAbsenceCommand(CreateAbsenceDTO absence) : IRequest<int>
{
    public CreateAbsenceDTO Absence { get; } = absence;
}