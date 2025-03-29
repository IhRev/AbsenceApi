using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Commands;

internal class AddAbsenceCommand(AbsenceDTO absence) : IRequest<int>
{
    public AbsenceDTO Absence { get; } = absence;
}