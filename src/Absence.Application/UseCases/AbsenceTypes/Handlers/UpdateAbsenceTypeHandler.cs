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

public class UpdateAbsenceTypeHandler(
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<UpdateAbsenceTypeCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(UpdateAbsenceTypeCommand request, CancellationToken cancellationToken)
    {
        var absenceType = await absenceTypesRepository.GetByIdAsync(request.AbsenceType.Id, cancellationToken);
        if (absenceType is null || absenceType.IsDeleted)
        {
            return new NotFound();
        }

        if (!await userOrganizationRoleRepository.HasPermission(request.OrganizationId, user.ShortId, Permissions.MANAGE_ABSENCE_TYPES, cancellationToken))
        {
            return new AccessDenied();
        }

        mapper.Map(request.AbsenceType, absenceType);
        absenceTypesRepository.Update(absenceType);
        await absenceTypesRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}