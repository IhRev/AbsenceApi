using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Commands;

public class AddHolidayCommand(CreateHolidayDTO holiday) : IRequest<OneOf<Success<int>, BadRequest, AccessDenied>>
{
    public CreateHolidayDTO Holiday { get; } = holiday;
}