using Absence.Domain.Entities;

namespace Absence.Domain.Repositories;

public interface IAbsenceEventRepository
{
    void Delete(AbsenceRequestEntity entity);

    void DeleteRange(IEnumerable<AbsenceRequestEntity> entities);

    ValueTask DisposeAsync();

    Task<List<AbsenceRequestEntity>> GetAsync(Func<IQueryable<AbsenceRequestEntity>, IQueryable<AbsenceRequestEntity>>[]? queries = null, CancellationToken cancellationToken = default);
    
    Task<AbsenceRequestEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
   
    Task<AbsenceRequestEntity?> GetFirstOrDefaultAsync(Func<IQueryable<AbsenceRequestEntity>, IQueryable<AbsenceRequestEntity>>[]? queries = null, CancellationToken cancellationToken = default);
   
    Task InsertAsync(AbsenceRequestEntity entity, CancellationToken cancellationToken = default);
    
    Task InsertRangeAsync(IEnumerable<AbsenceRequestEntity> entities, CancellationToken cancellationToken = default);
   
    Task SaveAsync(CancellationToken cancellationToken = default);
    
    void Update(AbsenceRequestEntity entity);
    
    void UpdateRange(IEnumerable<AbsenceRequestEntity> entities);
}