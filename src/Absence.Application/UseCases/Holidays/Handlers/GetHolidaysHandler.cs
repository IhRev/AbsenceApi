using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Application.UseCases.Holidays.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class GetHolidaysHandler(
    IRepository<HolidayEntity> holidayRepository,
    IMapper mapper
) : IRequestHandler<GetHolidaysQuery, IEnumerable<HolidayDTO>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<HolidayDTO>> Handle(GetHolidaysQuery request, CancellationToken cancellationToken)
    {
        var holidays = await _holidayRepository.GetAsync([], cancellationToken);
        return _mapper.Map<IEnumerable<HolidayDTO>>(holidays);
    }
}