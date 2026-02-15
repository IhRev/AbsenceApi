using Absence.Application.Common.Results;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Commands;

public record UpdateAbsenceTypeCommand(int OrganizationId, UpdateAbsenceTypeDTO AbsenceType)
    : IRequest<OneOf<Success, NotFound, BadRequest>>;