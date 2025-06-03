using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Results;

namespace Absence.Application.UseCases.Holidays.Commands;

public class DeleteHolidayCommand(int id) : IRequest<OneOf<Success, NotFound, AccessDenied>>
{
    public int Id { get; } = id;
}