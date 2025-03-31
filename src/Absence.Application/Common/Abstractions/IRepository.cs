namespace Absence.Application.Common.Abstractions;

public interface IRepository<TEntity> : IAsyncDisposable where TEntity : class
{
    void Delete(TEntity entity);

    Task DeleteByIdAsync(object id, CancellationToken cancellationToken = default);

    void DeleteRange(IEnumerable<TEntity> entities);

    Task<List<TEntity>> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>[] queries = null!, CancellationToken cancellationToken = default);
    
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
   
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
  
    Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
   
    void Update(TEntity entity);
    
    void UpdateRange(IEnumerable<TEntity> entities);

    Task SaveAsync(CancellationToken cancellationToken = default);
}