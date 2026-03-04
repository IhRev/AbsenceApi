using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using MediatR;

namespace Absence.Application.UseCases.AbsenceTypes.Queries;

public record GetAllAbsenceTypesQuery(int OrganizationId) 
    : IRequest<IEnumerable<AbsenceTypeDTO>>, IRequirePermission
{
    public string Permission => PermissionNames.VIEW_BASICS;
}