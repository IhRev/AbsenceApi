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

public class CreateAbsenceDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public int Type { get; set; }
    [Required]
    public DateTimeOffset StartDate { get; set; }
    [Required]
    public DateTimeOffset EndDate { get; set; }
    [Required]
    public int Organization { get; set; }
}

public static class AddAbsence
{
    public sealed class Command(CreateAbsenceDTO absence) : IRequest<OneOf<Success<int>, Success<string>, BadRequest>>
    {
        public CreateAbsenceDTO Absence { get; } = absence;
    }

    internal sealed class Handler(
        IRepository<AbsenceEntity> absenceRepository,
        IRepository<AbsenceTypeEntity> absenceTypesRepository,
        IRepository<AbsenceEventEntity> absenceEventRepository,
        IOrganizationUsersRepository organizationUserRepository,
        IAbsenceHolidayOverlapChecker overlapChecker,
        IMapper mapper,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success<int>, Success<string>, BadRequest>>
    {
        private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
        private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository = absenceTypesRepository;
        private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IAbsenceHolidayOverlapChecker _overlapChecker = overlapChecker;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;

        public async Task<OneOf<Success<int>, Success<string>, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
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

            if (request.Absence.StartDate > request.Absence.EndDate)
            {
                return new BadRequest("Start date must be before end date.");
            }

            if (await _overlapChecker.AbsenceOverlapsHolidayAsync(
                request.Absence.Organization,
                request.Absence.StartDate,
                request.Absence.EndDate,
                cancellationToken))
            {
                return new BadRequest("Absence overlaps a holiday.");
            }

            if (organizationUser.IsAdmin)
            {
                var absence = _mapper.Map<AbsenceEntity>(request.Absence);
                absence.UserId = _user.ShortId;
                await _absenceRepository.InsertAsync(absence, cancellationToken);
                await _absenceRepository.SaveAsync(cancellationToken);
                return new Success<int>(absence.Id);
            }

            var absenceEvent = _mapper.Map<AbsenceEventEntity>(request.Absence);
            absenceEvent.UserId = _user.ShortId;
            absenceEvent.AbsenceEventType = AbsenceEventType.CREATE;
            await _absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence create requested.");
        }
    }
}
