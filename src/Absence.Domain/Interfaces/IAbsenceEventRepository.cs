using Absence.Domain.Entities;

namespace Absence.Domain.Interfaces;

public interface IAbsenceEventRepository
{
    void Delete(AbsenceEventEntity entity);
    void DeleteRange(IEnumerable<AbsenceEventEntity> entities);
    ValueTask DisposeAsync();
    Task<List<AbsenceEventEntity>> GetAsync(Func<IQueryable<AbsenceEventEntity>, IQueryable<AbsenceEventEntity>>[] queries = null, CancellationToken cancellationToken = default);
    Task<AbsenceEventEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<AbsenceEventEntity?> GetFirstOrDefaultAsync(Func<IQueryable<AbsenceEventEntity>, IQueryable<AbsenceEventEntity>>[] queries = null, CancellationToken cancellationToken = default);
    Task InsertAsync(AbsenceEventEntity entity, CancellationToken cancellationToken = default);
    Task InsertRangeAsync(IEnumerable<AbsenceEventEntity> entities, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
    void Update(AbsenceEventEntity entity);
    void UpdateRange(IEnumerable<AbsenceEventEntity> entities);
}