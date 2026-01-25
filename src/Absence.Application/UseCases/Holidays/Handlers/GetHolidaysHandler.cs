using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Application.UseCases.Holidays.Queries;
using Absence.Domain.Entities;
using AutoMapper;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class GetHolidaysHandler(
    IRepository<HolidayEntity> holidayRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetHolidaysQuery, OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
{
    public async Task<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>> Handle(GetHolidaysQuery request, CancellationToken cancellationToken)
    {
        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }

        var holidays = await holidayRepository.GetAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId),
                q => q.Where(_ => _.Date > request.StartDate && _.Date < request.EndDate),
            ],
            cancellationToken
        );
        return new Success<IEnumerable<HolidayDTO>>(mapper.Map<IEnumerable<HolidayDTO>>(holidays));
    }
}