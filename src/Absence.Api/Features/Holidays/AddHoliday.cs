using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public class CreateHolidayDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required()]
    public required DateTimeOffset Date { get; set; }
    [Required()]
    public required int OrganizationId { get; set; }
}

public static class AddHoliday
{
    public sealed class Command(CreateHolidayDTO holiday) : IRequest<OneOf<Success<int>, BadRequest, AccessDenied>>
    {
        public CreateHolidayDTO Holiday { get; } = holiday;
    }

    internal sealed class Handler(
        IRepository<HolidayEntity> holidayRepository,
        IMapper mapper,
        IUser user,
        IRepository<OrganizationUserEntity> organizationUserRepository,
        IAbsenceHolidayOverlapChecker overlapChecker
    ) : IRequestHandler<Command, OneOf<Success<int>, BadRequest, AccessDenied>>
    {
        private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
        private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;
        private readonly IAbsenceHolidayOverlapChecker _overlapChecker = overlapChecker;

        public async Task<OneOf<Success<int>, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
               [
                   q => q.Where(_ => _.UserId == _user.ShortId),
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

            if (await _overlapChecker.HolidayOverlapsAbsenceAsync(
                request.Holiday.OrganizationId,
                request.Holiday.Date,
                cancellationToken))
            {
                return new BadRequest("Holiday overlaps an existing absence.");
            }

            var holiday = _mapper.Map<HolidayEntity>(request.Holiday);
            await _holidayRepository.InsertAsync(holiday, cancellationToken);
            await _holidayRepository.SaveAsync(cancellationToken);
            return new Success<int>(holiday.Id);
        }
    }
}
