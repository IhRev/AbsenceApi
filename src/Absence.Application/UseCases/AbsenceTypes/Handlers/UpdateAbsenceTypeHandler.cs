using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

public class UpdateAbsenceTypeHandler(
    IRepository<AbsenceTypeEntity> absenceTypesRepository,
    IMapper mapper
) : IRequestHandler<UpdateAbsenceTypeCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        UpdateAbsenceTypeCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var absenceType = await absenceTypesRepository.GetByIdAsync(request.AbsenceType.Id, cancellationToken);
        if (absenceType is null || absenceType.IsDeleted)
        {
            return new NotFound();
        }

        mapper.Map(request.AbsenceType, absenceType);
        absenceTypesRepository.Update(absenceType);
        await absenceTypesRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}