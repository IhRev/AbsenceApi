﻿using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Absences.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class GetUserAbsencesHandler(
    IRepository<AbsenceEntity> absenceRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetUserAbsencesQuery, OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>> Handle(GetUserAbsencesQuery request, CancellationToken cancellationToken)
    {
        var organizationUser = await _organizationUserRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.UserId == _user.ShortId && _.UserId == request.UserId),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId)
            ],
            cancellationToken
        );
        if (organizationUser is null)
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }
        if (_user.ShortId != request.UserId && !organizationUser.IsAdmin)
        {
            return new AccessDenied();
        }
 
        var absences = await _absenceRepository.GetAsync(
            [ 
                q => q.Where(_ => _.UserId == request.UserId),
                q => q.Where(_ => _.StartDate < request.EndDate && _.EndDate > request.StartDate),
                q => q.Where(_ => _.OrganizationId == request.OrganizationId),
            ], 
            cancellationToken
        );
        return new Success<IEnumerable<AbsenceDTO>>(_mapper.Map<IEnumerable<AbsenceDTO>>(absences));
    }
}