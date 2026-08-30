using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Application.Common.Interfaces;
using AutoMapper;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class EditHolidayHandler(
    IRepository<HolidayEntity> holidayRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IUser user,
    IMapper mapper,
    IAbsenceHolidayOverlapChecker overlapChecker
) : IRequestHandler<EditHolidayCommand, OneOf<Success, NotFound, AccessDenied, BadRequest>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IUser _user = user;
    private readonly IMapper _mapper = mapper;
    private readonly IAbsenceHolidayOverlapChecker _overlapChecker = overlapChecker;

    public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(EditHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await _holidayRepository.GetByIdAsync(request.Holiday.Id);
        if (holiday is null)
        {
            return new NotFound();
        }

        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == holiday.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new NotFound();
        }
        if (!organizationUser.IsAdmin)
        {
            return new AccessDenied();
        }

        if (await _overlapChecker.HolidayOverlapsAbsenceAsync(
            holiday.OrganizationId,
            request.Holiday.Date,
            cancellationToken))
        {
            return new BadRequest("Holiday overlaps an existing absence.");
        }

        holiday = _mapper.Map(request.Holiday, holiday);
        _holidayRepository.Update(holiday);
        await _holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}