using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class EditAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IRepository<AbsenceTypeEntity> absenceTypeRepository
) : IRequestHandler<EditAbsenceCommand, OneOf<Success, NotFound, BadRequest>>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypeRepository = absenceTypeRepository;

    public async Task<OneOf<Success, NotFound, BadRequest>> Handle(EditAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Absence.Id);
        if (absence is null)
        {
            return new NotFound();
        }

        absence.StartDate = request.Absence.StartDate;
        absence.EndDate = request.Absence.EndDate;
        absence.Name = request.Absence.Name;
        if (absence.AbsenceTypeId != request.Absence.Type)
        {
            var type = await _absenceTypeRepository.GetByIdAsync(request.Absence.Type);
            if (type == null)
            {
                return new BadRequest($"Type with id {request.Absence.Type} doesn't exist");
            }
            
            absence.AbsenceTypeId = type.Id;
        }

        _absenceRepository.Update(absence);
        await _absenceRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}