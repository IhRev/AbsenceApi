using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class GetAllAbsenceTypesHandler(
    IRepository<AbsenceTypeEntity> absenceTypeRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<GetAllAbsenceTypesQuery, OneOf<Success<IEnumerable<AbsenceTypeDTO>>, AccessDenied>>
{
    public async Task<OneOf<Success<IEnumerable<AbsenceTypeDTO>>, AccessDenied>> Handle(GetAllAbsenceTypesQuery request, CancellationToken cancellationToken = default)
    {
        if (!await userOrganizationRoleRepository.HasPermission(request.OrganizationId, user.ShortId, Permissions.VIEW, cancellationToken))
        {
            return new AccessDenied();
        }

        var absenceTypes = await absenceTypeRepository.GetAsync(
            new AbsenceTypeSpec(request.OrganizationId), 
            cancellationToken
        );
        return new Success<IEnumerable<AbsenceTypeDTO>>(mapper.Map<IEnumerable<AbsenceTypeDTO>>(absenceTypes));
    }
}