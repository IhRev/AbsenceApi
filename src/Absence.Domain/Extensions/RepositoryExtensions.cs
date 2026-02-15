using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;

namespace Absence.Domain.Extensions;

public static class RepositoryExtensions
{
    public static async Task<bool> BelongsToOrganization(
        this IRepository<DepartmentEntity> repository, 
        int organizationId, 
        int userId, 
        CancellationToken cancellationToken = default
    ) => (await repository.GetFirstOrDefaultAsync(
            new DepartmentSpec(organizationId, userId),
            cancellationToken
        )) != null;
}