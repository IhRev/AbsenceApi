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

public class EditHolidayDTO
{
    [Required()]
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required()]
    public required DateTimeOffset Date { get; set; }
}

public static class EditHoliday
{
    public sealed class Command(EditHolidayDTO holiday) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public EditHolidayDTO Holiday { get; } = holiday;
    }

    internal sealed class Handler(
        IRepository<HolidayEntity> holidayRepository,
        IRepository<OrganizationUserEntity> organizationUserRepository,
        IUser user,
        IMapper mapper,
        IAbsenceHolidayOverlapChecker overlapChecker
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
        private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
        private readonly IUser _user = user;
        private readonly IMapper _mapper = mapper;
        private readonly IAbsenceHolidayOverlapChecker _overlapChecker = overlapChecker;

        public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
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
}
