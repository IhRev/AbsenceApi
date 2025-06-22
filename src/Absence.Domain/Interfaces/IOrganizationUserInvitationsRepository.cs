using Absence.Domain.Entities;

namespace Absence.Domain.Interfaces;

public interface IOrganizationUserInvitationsRepository
{
    Task<List<OrganizationUserInvitationEntity>> GetAsync(Func<IQueryable<OrganizationUserInvitationEntity>, IQueryable<OrganizationUserInvitationEntity>>[] queries = null!, CancellationToken cancellationToken = default);

    void Delete(OrganizationUserInvitationEntity entity);

    void DeleteRange(IEnumerable<OrganizationUserInvitationEntity> entities);

    Task<OrganizationUserInvitationEntity?> GetFirstOrDefaultAsync(Func<IQueryable<OrganizationUserInvitationEntity>, IQueryable<OrganizationUserInvitationEntity>>[] queries = null!, CancellationToken cancellationToken = default);

    Task<OrganizationUserInvitationEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    Task InsertAsync(OrganizationUserInvitationEntity entity, CancellationToken cancellationToken = default);

    Task InsertRangeAsync(IEnumerable<OrganizationUserInvitationEntity> entities, CancellationToken cancellationToken = default);

    void Update(OrganizationUserInvitationEntity entity);

    void UpdateRange(IEnumerable<OrganizationUserInvitationEntity> entities);

    Task SaveAsync(CancellationToken cancellationToken = default);
}