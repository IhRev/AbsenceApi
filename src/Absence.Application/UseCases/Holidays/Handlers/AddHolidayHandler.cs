using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Holidays.Handlers;

internal class AddHolidayHandler(
    IRepository<HolidayEntity> holidayRepository,
    IMapper mapper,
    IUser user,
    IRepository<OrganizationUserEntity> organizationUserRepository
) : IRequestHandler<AddHolidayCommand, OneOf<Success<int>, BadRequest, AccessDenied>>
{
    public async Task<OneOf<Success<int>, BadRequest, AccessDenied>> Handle(AddHolidayCommand request, CancellationToken cancellationToken)
    {
        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
           [
               q => q.Where(_ => _.UserId == user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.Holiday.OrganizationId)
           ],
           cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.Holiday.OrganizationId} found.");
        }
        if (!organizationUser.IsAdmin)
        {
            return new AccessDenied();
        }

        var holiday = mapper.Map<HolidayEntity>(request.Holiday);
        await holidayRepository.InsertAsync(holiday, cancellationToken);
        await holidayRepository.SaveAsync(cancellationToken);
        return new Success<int>(holiday.Id);
    }
}