using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Common;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class AddAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IRepository<AbsenceTypeEntity> absenceTypesRepository, 
    IRepository<AbsenceEventTypeEntity> absenceEventTypeRepository,
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddAbsenceCommand, OneOf<Success<int>, Success<string>, BadRequest>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository = absenceTypesRepository;
    private readonly IRepository<AbsenceEventTypeEntity> _absenceEventTypeRepository = absenceEventTypeRepository;
    private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<int>, Success<string>, BadRequest>> Handle(AddAbsenceCommand request, CancellationToken cancellationToken)
    {
        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.Absence.Organization)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.Absence.Organization} found.");
        }

        var absenceType = await _absenceTypesRepository.GetByIdAsync(request.Absence.Type, cancellationToken);
        if (absenceType is null)
        {
            return new BadRequest($"No absence type with id {request.Absence.Type} found.");
        }

        if (organizationUser.IsAdmin)
        {
            var absence = _mapper.Map<AbsenceEntity>(request.Absence);
            absence.UserId = _user.ShortId;
            await _absenceRepository.InsertAsync(absence, cancellationToken);
            await _absenceRepository.SaveAsync(cancellationToken);
            return new Success<int>(absence.Id);
        }
        else
        {
            var absenceEvent = _mapper.Map<AbsenceEventEntity>(request.Absence);
            absenceEvent.UserId = _user.ShortId;
            var eventType = await _absenceEventTypeRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.Name == AbsenceEventType.CREATE)
                ],
                cancellationToken
            );
            absenceEvent.AbsenceEventTypeId = eventType!.Id;
            await _absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence create requested.");
        }
    }
}