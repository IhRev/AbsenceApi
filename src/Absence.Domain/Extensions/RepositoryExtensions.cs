using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;

namespace Absence.Domain.Extensions;

public static class RepositoryExtensions
{
    public static Task<bool> HasPermission(
        this IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
        int organizationId,
        int userId,
        string permission,
        CancellationToken cancellationToken = default
    ) => userOrganizationRoleRepository.AnyAsync(new PermissionSpec(permission, organizationId, userId), cancellationToken);
}