﻿using Absence.Application.Common.Results;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Absences.DTOs;

namespace Absence.Application.UseCases.Absences.Commands;

public class EditAbsenceCommand(EditAbsenceDTO absence) : IRequest<OneOf<Success, NotFound, BadRequest>>
{
    public EditAbsenceDTO Absence { get; } = absence;
}