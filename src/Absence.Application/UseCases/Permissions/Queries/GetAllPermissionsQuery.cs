using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Permissions.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Permissions.Queries;

public record GetAllPermissionsQuery(int OrganizationId) : IRequest<IEnumerable<PermissionDTO>>, IRequirePermission
{
    public string Permission => PermissionNames.MANAGE_PERMISSIONS;
}