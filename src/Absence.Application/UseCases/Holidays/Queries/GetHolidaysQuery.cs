using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Holidays.Queries;

public class GetHolidaysQuery : IRequest<IEnumerable<HolidayDTO>>;