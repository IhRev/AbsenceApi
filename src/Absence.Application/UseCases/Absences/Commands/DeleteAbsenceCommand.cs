using MediatR;

namespace Absence.Application.UseCases.Absences.Commands;

public class DeleteAbsenceCommand(int id) : IRequest
{
    public int Id { get; } = id;
}