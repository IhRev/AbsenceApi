using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public static class GetHolidays
{
    public sealed class Query(int organizationId, DateTimeOffset startDate, DateTimeOffset endDate)
        : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
    {
        public int OrganizationId { get; } = organizationId;
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
    }

    internal sealed class Handler(
        IRepository<HolidayEntity> holidayRepository,
        IRepository<OrganizationUserEntity> organizationUserRepository,
        IMapper mapper,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
    {
        private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
        private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;

        public async Task<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>> Handle(Query request, CancellationToken cancellationToken)
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
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId),
                    q => q.Where(_ => _.Date >= request.StartDate && _.Date <= request.EndDate),
                ],
                cancellationToken
            );
            return new Success<IEnumerable<HolidayDTO>>(_mapper.Map<IEnumerable<HolidayDTO>>(holidays));
        }
    }
}
