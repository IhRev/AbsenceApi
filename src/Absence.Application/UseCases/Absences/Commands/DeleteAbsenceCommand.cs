using MediatR;

namespace Absence.Application.UseCases.Absences.Commands;

internal class DeleteAbsenceCommand(int id) : IRequest
{
    public int Id { get; } = id;
}