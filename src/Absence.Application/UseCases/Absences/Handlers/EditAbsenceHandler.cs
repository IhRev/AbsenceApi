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

internal class EditAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IRepository<AbsenceTypeEntity> absenceTypeRepository,
    IUser user,
    IRepository<AbsenceEventTypeEntity> absenceEventTypeRepository,
    IRepository<AbsenceEventEntity> absenceEventRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IMapper mapper
) : IRequestHandler<EditAbsenceCommand, OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypeRepository = absenceTypeRepository;
    private readonly IRepository<AbsenceEventTypeEntity> _absenceEventTypeRepository = absenceEventTypeRepository;
    private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>> Handle(EditAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Absence.Id, cancellationToken);
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
                q => q.Where(_ => _.UserId == absence.UserId),
                q => q.Where(_ => _.OrganizationId == absence.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser!.IsAdmin)
        {
            if (absence.AbsenceTypeId != request.Absence.Type)
            {
                var type = await _absenceTypeRepository.GetByIdAsync(request.Absence.Type, cancellationToken);
                if (type is null)
                {
                    return new BadRequest($"Type with id {request.Absence.Type} doesn't exist");
                }

                absence = _mapper.Map(request.Absence, absence);
            }

            _absenceRepository.Update(absence);
            await _absenceRepository.SaveAsync(cancellationToken);

            return new Success<string>("Absence updated.");
        }
        else
        {
            var absenceEvent = _mapper.Map<AbsenceEventEntity>(request.Absence);
            var eventType = await _absenceEventTypeRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.Name == AbsenceEventType.CREATE)
                ],
                cancellationToken
            );
            absenceEvent.AbsenceEventTypeId = eventType!.Id;
            absenceEvent.OrganizationId = organizationUser.OrganizationId;
            absenceEvent.UserId = organizationUser.UserId;
            await _absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence update requested.");
        }
    }
}