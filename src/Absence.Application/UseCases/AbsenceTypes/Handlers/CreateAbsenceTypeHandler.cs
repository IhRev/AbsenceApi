using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Handlers;

internal class CreateAbsenceTypeHandler(IRepository<AbsenceTypeEntity> absenceTypesRepository, IMapper mapper)
    : IRequestHandler<CreateAbsenceTypeCommand, int>
{
    public async Task<int> Handle(CreateAbsenceTypeCommand request, CancellationToken cancellationToken = default)
    {
        var absenceType = mapper.Map<AbsenceTypeEntity>(request.AbsenceType);
        absenceType.OrganizationId = request.OrganizationId;
        await absenceTypesRepository.InsertAsync(absenceType, cancellationToken);
        await absenceTypesRepository.SaveAsync(cancellationToken);

        return absenceType.Id;
    }
}