using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetUsersAbsences
{
    public sealed class Query(DateTimeOffset startDate, DateTimeOffset endDate, int organizationId, List<int> userIds)
        : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
    {
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
        public int OrganizationId { get; } = organizationId;
        public List<int> UserIds { get; } = userIds;
    }

    internal sealed class Handler(
        IRepository<AbsenceEntity> absenceRepository,
        IOrganizationUsersRepository organizationUserRepository,
        IMapper mapper,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
    {
        private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUser _user = user;

        public async Task<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>> Handle(Query request, CancellationToken cancellationToken)
        {
            var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
                [
                    q => q.Where(_ => _.UserId == _user.ShortId),
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId)
                ],
                cancellationToken
            );
            if (organizationUser is null)
            {
                return new BadRequest($"No organization with id {request.OrganizationId} found.");
            }
            if (!organizationUser.IsAdmin)
            {
                return new AccessDenied();
            }

            var absences = await _absenceRepository.GetAsync(
                [
                    q => q.Where(_ => request.UserIds.Contains(_.UserId)),
                    q => q.Where(_ => _.StartDate < request.EndDate && _.EndDate > request.StartDate),
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId),
                ],
                cancellationToken
            );
            return new Success<IEnumerable<AbsenceDTO>>(_mapper.Map<IEnumerable<AbsenceDTO>>(absences));
        }
    }
}
