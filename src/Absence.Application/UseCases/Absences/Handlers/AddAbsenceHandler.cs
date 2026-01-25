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

internal class AddAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IRepository<AbsenceTypeEntity> absenceTypesRepository, 
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddAbsenceCommand, OneOf<Success<int>, Success<string>, BadRequest>>
{
    public async Task<OneOf<Success<int>, Success<string>, BadRequest>> Handle(AddAbsenceCommand request, CancellationToken cancellationToken)
    {
        var organizationUser = await organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.Absence.Organization)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.Absence.Organization} found.");
        }

        var absenceType = await absenceTypesRepository.GetByIdAsync(request.Absence.Type, cancellationToken);
        if (absenceType is null || absenceType.OrganizationId != organizationUser.OrganizationId)
        {
            return new BadRequest($"No absence type with id {request.Absence.Type} found.");
        }

        if (organizationUser.IsAdmin)
        {
            var absence = mapper.Map<AbsenceEntity>(request.Absence);
            absence.UserId = user.ShortId;
            await absenceRepository.InsertAsync(absence, cancellationToken);
            await absenceRepository.SaveAsync(cancellationToken);
            return new Success<int>(absence.Id);
        }
        else
        {
            var absenceEvent = mapper.Map<AbsenceEventEntity>(request.Absence);
            absenceEvent.UserId = user.ShortId;
            absenceEvent.AbsenceEventType = AbsenceEventType.CREATE;
            await absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence create requested.");
        }
    }
}