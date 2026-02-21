using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

public class DeleteAbsenceTypeHandler(
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IUser user
) : IRequestHandler<DeleteAbsenceTypeCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(DeleteAbsenceTypeCommand request, CancellationToken cancellationToken)
    {
        var absenceType = await absenceTypesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (absenceType is null || absenceType.IsDeleted)
        {
            return new NotFound();
        }

        if (!await userOrganizationRoleRepository.HasPermission(absenceType.OrganizationId, user.ShortId, Permissions.MANAGE_ABSENCE_TYPES, cancellationToken))
        {
            return new AccessDenied();
        }

        absenceTypesRepository.Delete(absenceType);
        await absenceTypesRepository.SaveAsync(cancellationToken);
        return new Success();
    }
}