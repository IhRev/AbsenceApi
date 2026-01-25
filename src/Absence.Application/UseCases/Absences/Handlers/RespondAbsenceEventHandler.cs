using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Common;
using AutoMapper;
using Absence.Domain.Repositories;

namespace Absence.Application.UseCases.Absences.Handlers;

public class RespondAbsenceEventHandler(
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IRepository<AbsenceEntity> absenceRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<RespondAbsenceEventCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(RespondAbsenceEventCommand request, CancellationToken cancellationToken)
    {
        var absenceEvent = await absenceEventRepository.GetByIdAsync(request.Id, cancellationToken);
        if (absenceEvent is null)
        {
            return new NotFound();
        }

        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
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
            await absenceRepository.SaveAsync(cancellationToken);
        }

        absenceEventRepository.Delete(absenceEvent);
        await absenceEventRepository.SaveAsync(cancellationToken);
        return new Success();
    }

    private Task AddAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
    {
        var absence = mapper.Map<AbsenceEntity>(absenceEvent);
        return absenceRepository.InsertAsync(absence, cancellationToken);
    }

    private async Task UpdateAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
    {
        var absence = await absenceRepository.GetByIdAsync(absenceEvent.AbsenceId!, cancellationToken);
        absence = mapper.Map(absenceEvent, absence);
        absenceRepository.Update(absence!);
    }

    private async Task DeleteAbsence(AbsenceEventEntity absenceEvent, CancellationToken cancellationToken = default)
    {
        var absence = await absenceRepository.GetByIdAsync(absenceEvent.AbsenceId!, cancellationToken);
        absenceRepository.Delete(absence!);
    }
} 