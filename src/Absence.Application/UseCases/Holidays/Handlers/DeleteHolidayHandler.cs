using Absence.Application.UseCases.Absences.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Application.UseCases.Holidays.Commands;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class DeleteHolidayHandler(IRepository<HolidayEntity> holidayRepository) : IRequestHandler<DeleteHolidayCommand, OneOf<Success, NotFound>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;

    public async Task<OneOf<Success, NotFound>> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await _holidayRepository.GetByIdAsync(request.Id);
        if (holiday is null)
        {
            return new NotFound();
        }

        _holidayRepository.Delete(holiday);
        await _holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}