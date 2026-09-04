using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public static class DeleteHoliday
{
    public sealed class Command(int id) : IRequest<OneOf<Success, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
    }

    internal sealed class Handler(
        IRepository<HolidayEntity> holidayRepository,
        IRepository<OrganizationUserEntity> organizationUserRepository,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied>>
    {
        private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
        private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
        private readonly IUser _user = user;

        public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
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
}
