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

internal class EditAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IRepository<AbsenceTypeEntity> absenceTypeRepository,
    IUser user,
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper
) : IRequestHandler<EditAbsenceCommand, OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>
{
    public async Task<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>> Handle(EditAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await absenceRepository.GetByIdAsync(request.Absence.Id, cancellationToken);
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
                q => q.Where(_ => _.UserId == absence.UserId),
                q => q.Where(_ => _.OrganizationId == absence.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser!.IsAdmin)
        {
            if (absence.AbsenceTypeId != request.Absence.Type)
            {
                var type = await absenceTypeRepository.GetByIdAsync(request.Absence.Type, cancellationToken);
                if (type is null || type.OrganizationId != organizationUser.OrganizationId)
                {
                    return new BadRequest($"Type with id {request.Absence.Type} doesn't exist");
                }
            }
            absence = mapper.Map(request.Absence, absence);

            absenceRepository.Update(absence);
            await absenceRepository.SaveAsync(cancellationToken);

            return new Success<string>("Absence updated.");
        }
        else
        {
            var absenceEvent = mapper.Map<AbsenceEventEntity>(request.Absence);
            absenceEvent.AbsenceEventType = AbsenceEventType.CREATE;
            absenceEvent.OrganizationId = organizationUser.OrganizationId;
            absenceEvent.UserId = organizationUser.UserId;
            await absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence update requested.");
        }
    }
}