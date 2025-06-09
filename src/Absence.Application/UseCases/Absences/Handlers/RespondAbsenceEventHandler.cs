using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Interfaces;
using Absence.Domain.Entities;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Common;
using AutoMapper;

namespace Absence.Application.UseCases.Absences.Handlers;

public class RespondAbsenceEventHandler(
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IRepository<AbsenceEventTypeEntity> absenceEventTypesRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IRepository<AbsenceEntity> absenceRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<RespondAbsenceEventCommand, OneOf<Success, NotFound, AccessDenied>>
{
    private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
    private readonly IRepository<AbsenceEventTypeEntity> _absenceEventTypesRepository = absenceEventTypesRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IUser _user = user;
    private readonly IMapper _mapper = mapper;

    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(RespondAbsenceEventCommand request, CancellationToken cancellationToken)
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
            var eventType = await _absenceEventTypesRepository.GetByIdAsync(absenceEvent.AbsenceEventTypeId, cancellationToken);
            switch (eventType!.Name)
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
                    throw new ArgumentException($"Incorrect event type id {absenceEvent.AbsenceEventTypeId}");
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
        var absence = await _absenceRepository.GetByIdAsync(absenceEvent.AbsenceId!, cancellationToken);
        absence = _mapper.Map(absenceEvent, absence);
        _absenceRepository.Update(absence!);
    }

    private async Task DeleteAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
    {
        var absence = await _absenceRepository.GetByIdAsync(absenceEvent.AbsenceId!, cancellationToken);
        _absenceRepository.Delete(absence!);
    }
} 