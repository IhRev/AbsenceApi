using System.ComponentModel.DataAnnotations;
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

public class EditAbsenceDTO
{
    [Required]
    public required int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public required int Type { get; set; }
    [Required]
    public required DateTimeOffset StartDate { get; set; }
    [Required]
    public required DateTimeOffset EndDate { get; set; }
}

public static class EditAbsence
{
    public sealed class Command(EditAbsenceDTO absence) : IRequest<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>
    {
        public EditAbsenceDTO Absence { get; } = absence;
    }

    internal sealed class Handler(
        IRepository<AbsenceEntity> absenceRepository,
        IRepository<AbsenceTypeEntity> absenceTypeRepository,
        IUser user,
        IRepository<AbsenceEventEntity> absenceEventRepository,
        IOrganizationUsersRepository organizationUserRepository,
        IAbsenceHolidayOverlapChecker overlapChecker,
        IMapper mapper
    ) : IRequestHandler<Command, OneOf<Success<string>, NotFound, BadRequest, AccessDenied>>
    {
        private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
        private readonly IRepository<AbsenceTypeEntity> _absenceTypeRepository = absenceTypeRepository;
        private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IAbsenceHolidayOverlapChecker _overlapChecker = overlapChecker;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;

        public async Task<OneOf<Success<string>, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
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
            if (organizationUser is null)
            {
                return new AccessDenied();
            }

            if (request.Absence.StartDate > request.Absence.EndDate)
            {
                return new BadRequest("Start date must be before end date.");
            }

            if (await _overlapChecker.AbsenceOverlapsHolidayAsync(
                absence.OrganizationId,
                request.Absence.StartDate,
                request.Absence.EndDate,
                cancellationToken))
            {
                return new BadRequest("Absence overlaps a holiday.");
            }

            if (organizationUser.IsAdmin)
            {
                if (absence.AbsenceTypeId != request.Absence.Type)
                {
                    var type = await _absenceTypeRepository.GetByIdAsync(request.Absence.Type, cancellationToken);
                    if (type is null)
                    {
                        return new BadRequest($"Type with id {request.Absence.Type} doesn't exist");
                    }
                }
                absence = _mapper.Map(request.Absence, absence);

                _absenceRepository.Update(absence);
                await _absenceRepository.SaveAsync(cancellationToken);

                return new Success<string>("Absence updated.");
            }

            var absenceEvent = _mapper.Map<AbsenceEventEntity>(request.Absence);
            absenceEvent.AbsenceEventType = AbsenceEventType.UPDATE;
            absenceEvent.OrganizationId = organizationUser.OrganizationId;
            absenceEvent.UserId = organizationUser.UserId;
            await _absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence update requested.");
        }
    }
}
