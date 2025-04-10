﻿using Absence.Application.UseCases.Absences.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;

namespace Absence.Application.UseCases.Absences.Handlers;

internal class EditAbsenceHandler(
    IRepository<AbsenceEntity> absenceRepository, 
    IRepository<AbsenceTypeEntity> absenceTypeRepository
) : IRequestHandler<EditAbsenceCommand>
{
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypeRepository = absenceTypeRepository;

    public async Task Handle(EditAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Absence.Id);
        if (absence is null)
        {
            return;
        }

        absence.StartDate = request.Absence.StartDate;
        absence.EndDate = request.Absence.EndDate;
        absence.Name = request.Absence.Name;
        if (absence.AbsenceTypeId != request.Absence.Type)
        {
            var type = await _absenceTypeRepository.GetByIdAsync(request.Absence.Type);
            if (type == null)
            {
                return;
            }
            
            absence.AbsenceTypeId = type.Id;
        }

        _absenceRepository.Update(absence);
        await _absenceRepository.SaveAsync(cancellationToken);
    }
}