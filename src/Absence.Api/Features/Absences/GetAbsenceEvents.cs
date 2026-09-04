using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Absences;

public static class GetAbsenceEvents
{
    public sealed class Query(int organizationId) : IRequest<OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>>
    {
        public int OrganizationId { get; } = organizationId;
    }

    internal sealed class Handler(
        IOrganizationUsersRepository organizationUserRepository,
        IAbsenceEventRepository absenceEventRepository,
        IUser user,
        IMapper mapper
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>>
    {
        private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
        private readonly IAbsenceEventRepository _absenceEventRepository = absenceEventRepository;
        private readonly IUser _user = user;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>> Handle(Query request, CancellationToken cancellationToken)
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

            var events = await _absenceEventRepository.GetAsync(
                [
                    q => q.Where(_ => _.OrganizationId == request.OrganizationId)
                ]
            );
            return new Success<IEnumerable<AbsenceEventDTO>>(_mapper.Map<IEnumerable<AbsenceEventDTO>>(events));
        }
    }
}
