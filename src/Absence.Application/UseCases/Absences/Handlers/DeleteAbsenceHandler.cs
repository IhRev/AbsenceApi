using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Common;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class DeleteAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<DeleteAbsenceCommand, OneOf<Success<string>, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success<string>, NotFound, AccessDenied>> Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await absenceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (absence is null)
        {
            return new NotFound();
        }
        if (absence.UserId != user.ShortId)
        {
            return new AccessDenied();
        }

        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
                q => q.Where(_ => _.OrganizationId == absence.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser!.IsAdmin)
        {
            absenceRepository.Delete(absence);
            await absenceRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence deleted.");
        }
        else
        {
            var absenceEvent = mapper.Map<AbsenceEventEntity>(absence);
            absenceEvent.AbsenceEventType = AbsenceEventType.DELETE;
            await absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence delete requested.");
        }
    }
}