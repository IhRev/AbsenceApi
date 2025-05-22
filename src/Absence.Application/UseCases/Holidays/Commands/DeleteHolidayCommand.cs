using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Commands;

public class DeleteHolidayCommand(int id) : IRequest<OneOf<Success, NotFound>>
{
    public int Id { get; } = id;
}