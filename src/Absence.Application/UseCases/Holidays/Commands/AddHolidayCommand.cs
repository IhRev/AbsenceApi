using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Holidays.Commands;

public class AddHolidayCommand(CreateHolidayDTO holiday) : IRequest<int>
{
    public CreateHolidayDTO Holiday { get; } = holiday;
}