using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class CreateAbsenceTypeHandler(
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<CreateAbsenceTypeCommand, OneOf<Success<int>, AccessDenied>>
{
    public async Task<OneOf<Success<int>, AccessDenied>> Handle(CreateAbsenceTypeCommand request, CancellationToken cancellationToken)
    {
        if (!await userOrganizationRoleRepository.HasPermission(request.OrganizationId, user.ShortId, Permissions.MANAGE_ABSENCE_TYPES, cancellationToken))
        {
            return new AccessDenied();
        }

        var absenceType = mapper.Map<AbsenceTypeEntity>(request.AbsenceType);
        absenceType.OrganizationId = request.OrganizationId;
        await absenceTypesRepository.InsertAsync(absenceType, cancellationToken);
        await absenceTypesRepository.SaveAsync(cancellationToken);

        return new Success<int>(absenceType.Id);
    }
}