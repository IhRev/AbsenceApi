using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Application.UseCases.Holidays.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;

namespace Absence.Application.UseCases.Holidays.Handlers;

public class GetHolidaysHandler(
    IRepository<HolidayEntity> holidayRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetHolidaysQuery, OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>> Handle(GetHolidaysQuery request, CancellationToken cancellationToken)
    {
        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }

        var holidays = await _holidayRepository.GetAsync(
            [
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        return new Success<IEnumerable<HolidayDTO>>(_mapper.Map<IEnumerable<HolidayDTO>>(holidays));
    }
}