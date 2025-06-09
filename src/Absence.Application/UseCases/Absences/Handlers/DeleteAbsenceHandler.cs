using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Common;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class DeleteAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IRepository<AbsenceEventTypeEntity> absenceEventTypeRepository,
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IUser user
) : IRequestHandler<DeleteAbsenceCommand, OneOf<Success<string>, NotFound, AccessDenied>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IRepository<AbsenceEventTypeEntity> _absenceEventTypeRepository = absenceEventTypeRepository;
    private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<string>, NotFound, AccessDenied>> Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (absence is null)
        {
            return new NotFound();
        }
        if (absence.UserId != _user.ShortId)
        {
            return new AccessDenied();
        }

        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == absence.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser!.IsAdmin)
        {
            _absenceRepository.Delete(absence);
            await _absenceRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence deleted.");
        }
        else
        {
            var absenceEvent = new AbsenceEventEntity()
            {
                OrganizationId = organizationUser.OrganizationId,
                UserId = organizationUser.UserId
            };
            var eventType = await _absenceEventTypeRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.Name == AbsenceEventType.DELETE)
                ],
                cancellationToken
            );
            absenceEvent.AbsenceEventTypeId = eventType!.Id;
            await _absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence delete requested.");
        }
    }
}