using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Application.Common.Results;
using Absence.Application.Common.Interfaces;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class DeleteHolidayHandler(
    IRepository<HolidayEntity> holidayRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IUser user
) : IRequestHandler<DeleteHolidayCommand, OneOf<Success, NotFound, AccessDenied>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await _holidayRepository.GetByIdAsync(request.Id);
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

        _holidayRepository.Delete(holiday);
        await _holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}