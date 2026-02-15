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
    IRepository<DepartmentEntity> departmentRepository,
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<UpdateAbsenceTypeCommand, OneOf<Success, NotFound, BadRequest>>
{
    public async Task<OneOf<Success, NotFound, BadRequest>> Handle(UpdateAbsenceTypeCommand request, CancellationToken cancellationToken)
    {
        if (!await departmentRepository.BelongsToOrganization(request.OrganizationId, user.ShortId))
        {
            return new BadRequest($"No organization with id {request.OrganizationId} found.");
        }

        var absenceType = await absenceTypesRepository.GetByIdAsync(request.AbsenceType.Id, cancellationToken);
        if (absenceType is null || absenceType.IsDeleted)
        {
            return new NotFound(;
        }

        mapper.Map(request.AbsenceType, absenceType);
        absenceTypesRepository.Update(absenceType);
        await absenceTypesRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}