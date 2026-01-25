using Absence.Domain.Entities;

namespace Absence.Domain.Repositories;

public interface IOrganizationUsersRepository
{
    Task<List<OrganizationUserEntity>> GetAsync(Func<IQueryable<OrganizationUserEntity>, IQueryable<OrganizationUserEntity>>[] queries = null!, CancellationToken cancellationToken = default);

    void Delete(OrganizationUserEntity entity);

    void DeleteRange(IEnumerable<OrganizationUserEntity> entities);

    Task<OrganizationUserEntity?> GetFirstOrDefaultAsync(Func<IQueryable<OrganizationUserEntity>, IQueryable<OrganizationUserEntity>>[] queries = null!, CancellationToken cancellationToken = default);

    Task<OrganizationUserEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    Task InsertAsync(OrganizationUserEntity entity, CancellationToken cancellationToken = default);

    Task InsertRangeAsync(IEnumerable<OrganizationUserEntity> entities, CancellationToken cancellationToken = default);

    void Update(OrganizationUserEntity entity);

    void UpdateRange(IEnumerable<OrganizationUserEntity> entities);

    Task SaveAsync(CancellationToken cancellationToken = default);
}