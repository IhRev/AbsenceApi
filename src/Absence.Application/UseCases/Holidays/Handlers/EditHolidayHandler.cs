using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class EditHolidayHandler(IRepository<HolidayEntity> holidayRepository) : IRequestHandler<EditHolidayCommand, OneOf<Success, NotFound, BadRequest>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;

    public async Task<OneOf<Success, NotFound, BadRequest>> Handle(EditHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await _holidayRepository.GetByIdAsync(request.Holiday.Id);
        if (holiday is null)
        {
            return new NotFound();
        }

        holiday.StartDate = request.Holiday.StartDate;
        holiday.EndDate = request.Holiday.EndDate;
        holiday.Name = request.Holiday.Name;

        _holidayRepository.Update(holiday);
        await _holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}