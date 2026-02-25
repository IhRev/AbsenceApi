using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Commands;

public record CreateAbsenceTypeCommand(int OrganizationId, CreateAbsenceTypeDTO AbsenceType)
    : IRequest<int>, IRequirePermission
{
    public string Permission => Permissions.MANAGE_ABSENCE_TYPES;
}