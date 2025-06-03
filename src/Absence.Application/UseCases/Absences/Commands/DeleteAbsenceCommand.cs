using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Commands;

public class DeleteAbsenceCommand(int id) : IRequest<OneOf<Success, NotFound, AccessDenied>>
{
    public int Id { get; } = id;
}