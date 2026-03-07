using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

public class DeleteAbsenceTypeHandler(IRepository<AbsenceTypeEntity> absenceTypesRepository) 
    : IRequestHandler<DeleteAbsenceTypeCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        DeleteAbsenceTypeCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var absenceType = await absenceTypesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (absenceType is null)
        {
            return new NotFound();
        }

        absenceTypesRepository.Delete(absenceType);
        await absenceTypesRepository.SaveAsync(cancellationToken);
        return new Success();
    }
}