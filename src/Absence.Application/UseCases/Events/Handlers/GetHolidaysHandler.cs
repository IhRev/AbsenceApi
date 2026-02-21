using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Application.UseCases.Holidays.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class GetHolidaysHandler(
    IRepository<EventEntity> holidayRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetEventsQuery, OneOf<Success<IEnumerable<HolidayDTO>>, AccessDenied>>
{
    public async Task<OneOf<Success<IEnumerable<HolidayDTO>>, AccessDenied>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        if (!await userOrganizationRoleRepository.HasPermission(request.OrganizationId, user.ShortId, Permissions.VIEW, cancellationToken))
        {
            return new AccessDenied();
        }

        var holidays = await holidayRepository.GetAsync(new EventsSpec(request.OrganizationId, request.StartDate, request.EndDate));
        return new Success<IEnumerable<HolidayDTO>>(mapper.Map<IEnumerable<HolidayDTO>>(holidays));
    }
}