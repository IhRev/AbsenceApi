using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Holidays.Handlers;

internal class AddHolidayHandler(
    IRepository<HolidayEntity> holidayRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddHolidayCommand, int>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<int> Handle(AddHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = _mapper.Map<HolidayEntity>(request.Holiday);
        await _holidayRepository.InsertAsync(holiday, cancellationToken);
        await _holidayRepository.SaveAsync(cancellationToken);
        return holiday.Id;
    }
}