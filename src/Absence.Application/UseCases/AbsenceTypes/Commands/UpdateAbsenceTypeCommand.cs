using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Commands;

public record UpdateAbsenceTypeCommand(int OrganizationId, UpdateAbsenceTypeDTO AbsenceType)
    : IRequest<OneOf<Success, NotFound>>, IRequirePermission
{
    public string Permission => Permissions.MANAGE_ABSENCE_TYPES;
}