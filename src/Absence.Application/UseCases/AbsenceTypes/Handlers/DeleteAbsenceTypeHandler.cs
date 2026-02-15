using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

public class DeleteAbsenceTypeHandler(
    IRepository<DepartmentEntity> departmentRepository,
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IUser user
) : IRequestHandler<DeleteAbsenceTypeCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(DeleteAbsenceTypeCommand request, CancellationToken cancellationToken)
    {
        var absenceType = await absenceTypesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (absenceType is null || absenceType.IsDeleted)
        {
            return new NotFound();
        }

        if (!await departmentRepository.BelongsToOrganization(absenceType.OrganizationId, user.ShortId))
        {
            return new NotFound();
        }

        absenceTypesRepository.Delete(absenceType);
        await absenceTypesRepository.SaveAsync(cancellationToken);
        return new Success();
    }
}