using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Common;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class RespondAbsenceEvent
{
    public sealed class Command(int id, bool accepted) : IRequest<OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        public int Id { get; } = id;
        public bool Accepted { get; } = accepted;
    }

    internal sealed class Handler(
        IRepository<AbsenceEventEntity> absenceEventRepository,
        IOrganizationUsersRepository organizationUserRepository,
        IRepository<AbsenceEntity> absenceRepository,
        IAbsenceHolidayOverlapChecker overlapChecker,
        IUser user,
        IMapper mapper
    ) : IRequestHandler<Command, OneOf<Success, NotFound, AccessDenied, BadRequest>>
    {
        private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
        private readonly IAbsenceHolidayOverlapChecker _overlapChecker = overlapChecker;
        private readonly IUser _user = user;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<Success, NotFound, AccessDenied, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var absenceEvent = await _absenceEventRepository.GetByIdAsync(request.Id, cancellationToken);
            if (absenceEvent is null)
            {
                return new NotFound();
            }

            var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.UserId == _user.ShortId),
                    q => q.Where(_ => _.OrganizationId == absenceEvent.OrganizationId)
                ],
                cancellationToken
            );
            if (organizationUser is null || !organizationUser.IsAdmin)
            {
                return new AccessDenied();
            }

            if (request.Accepted)
            {
                if (absenceEvent.AbsenceEventType is AbsenceEventType.CREATE or AbsenceEventType.UPDATE)
                {
                    if (absenceEvent.StartDate > absenceEvent.EndDate)
                    {
                        return new BadRequest("Start date must be before end date.");
                    }

                    if (await _overlapChecker.AbsenceOverlapsHolidayAsync(
                        absenceEvent.OrganizationId,
                        absenceEvent.StartDate,
                        absenceEvent.EndDate,
                        cancellationToken))
                    {
                        return new BadRequest("Absence overlaps a holiday.");
                    }
                }

                switch (absenceEvent.AbsenceEventType)
                {
                    case AbsenceEventType.CREATE:
                        await AddAbsence(absenceEvent, cancellationToken);
                        break;
                    case AbsenceEventType.UPDATE:
                        await UpdateAbsence(absenceEvent, cancellationToken);
                        break;
                    case AbsenceEventType.DELETE:
                        await DeleteAbsence(absenceEvent, cancellationToken);
                        break;
                    default:
                        throw new ArgumentException($"Incorrect event type {absenceEvent.AbsenceEventType}");
                }
                await _absenceRepository.SaveAsync(cancellationToken);
            }

            _absenceEventRepository.Delete(absenceEvent);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success();
        }

        private Task AddAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            var absence = _mapper.Map<AbsenceEntity>(absenceEvent);
            return _absenceRepository.InsertAsync(absence, cancellationToken);
        }

        private async Task UpdateAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            if (absenceEvent.AbsenceId is not int absenceId)
            {
                return;
            }

            var absence = await _absenceRepository.GetByIdAsync(absenceId, cancellationToken);
            if (absence is null)
            {
                return;
            }

            absence = _mapper.Map(absenceEvent, absence);
            _absenceRepository.Update(absence);
        }

        private async Task DeleteAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
        {
            if (absenceEvent.AbsenceId is not int absenceId)
            {
                return;
            }

            var absence = await _absenceRepository.GetByIdAsync(absenceId, cancellationToken);
            if (absence is null)
            {
                return;
            }

            _absenceRepository.Delete(absence);
        }
    }
}
