using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Commands;

public class EditAbsenceCommand(AbsenceDTO absence) : IRequest
{
    public AbsenceDTO Absence { get; } = absence;
}