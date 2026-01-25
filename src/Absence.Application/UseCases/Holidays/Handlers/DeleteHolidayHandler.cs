using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Application.Common.Results;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class DeleteHolidayHandler(
    IRepository<HolidayEntity> holidayRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IUser user
) : IRequestHandler<DeleteHolidayCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await holidayRepository.GetByIdAsync(request.Id);
        if (holiday is null)
        {
            return new NotFound();
        }

        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
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

        holidayRepository.Delete(holiday);
        await holidayRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}