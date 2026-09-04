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

public static class DeleteAbsence
{
    public sealed class Command(int id) : IRequest<OneOf<Success<string>, NotFound, AccessDenied>>
    {
        public int Id { get; } = id;
    }

    internal sealed class Handler(
        IRepository<AbsenceEntity> absenceRepository,
        IOrganizationUsersRepository organizationUserRepository,
        IRepository<AbsenceEventEntity> absenceEventRepository,
        IUser user,
        IMapper mapper
    ) : IRequestHandler<Command, OneOf<Success<string>, NotFound, AccessDenied>>
    {
        private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IRepository<AbsenceEventEntity> _absenceEventRepository = absenceEventRepository;
        private readonly IUser _user = user;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<Success<string>, NotFound, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
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
            if (organizationUser is null)
            {
                return new AccessDenied();
            }
            if (organizationUser.IsAdmin)
            {
                _absenceRepository.Delete(absence);
                await _absenceRepository.SaveAsync(cancellationToken);
                return new Success<string>("Absence deleted.");
            }

            var absenceEvent = _mapper.Map<AbsenceEventEntity>(absence);
            absenceEvent.AbsenceEventType = AbsenceEventType.DELETE;
            await _absenceEventRepository.InsertAsync(absenceEvent, cancellationToken);
            await _absenceEventRepository.SaveAsync(cancellationToken);
            return new Success<string>("Absence delete requested.");
        }
    }
}
