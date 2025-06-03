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
    IRepository<AbsenceStatusEntity> absenceStatusRepository,
    IRepository<OrganizationUserEntity> organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddAbsenceCommand, OneOf<Success<int>, BadRequest>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository = absenceTypesRepository;
    private readonly IRepository<AbsenceStatusEntity> _absenceStatusRepository = absenceStatusRepository;
    private readonly IRepository<OrganizationUserEntity> _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<int>, BadRequest>> Handle(AddAbsenceCommand request, CancellationToken cancellationToken)
    {
        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId),
                q => q.Where(_ => _.OrganizationId == request.Absence.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.Absence.OrganizationId} found.");
        }

        var absenceType = await _absenceTypesRepository.GetByIdAsync(request.Absence.Type);
        if (absenceType is null)
        {
            return new BadRequest($"No absence type with id {request.Absence.Type} found.");
        }

        var absence = _mapper.Map<AbsenceEntity>(request.Absence);
        absence.UserId = _user.ShortId;
        var status = organizationUser.IsAdmin ? AbsenceStatus.ACCEPTED : AbsenceStatus.PENDING;
        absence.AbsenceStatusId = (await _absenceStatusRepository.GetFirstOrDefaultAsync([q => q.Where(_ => _.Name == status)], cancellationToken))!.Id;
        await _absenceRepository.InsertAsync(absence, cancellationToken);
        await _absenceRepository.SaveAsync(cancellationToken);
        return new Success<int>(absence.Id);
    }
}