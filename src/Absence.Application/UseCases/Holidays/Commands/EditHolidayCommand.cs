using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Commands;

public class EditHolidayCommand(EditHolidayDTO holiday) : IRequest<OneOf<Success, NotFound, BadRequest>>
{
    public EditHolidayDTO Holiday { get; } = holiday;
}