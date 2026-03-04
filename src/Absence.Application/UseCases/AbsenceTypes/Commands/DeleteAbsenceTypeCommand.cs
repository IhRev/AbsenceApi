using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Commands;

public record DeleteAbsenceTypeCommand(int OrganizationId, int Id) 
    : IRequest<OneOf<Success, NotFound>>, IRequirePermission
{
    public string Permission => PermissionNames.MANAGE_ABSENCE_TYPES;
}