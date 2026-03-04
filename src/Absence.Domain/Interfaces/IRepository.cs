using Ardalis.Specification;

namespace Absence.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    void Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    void DeleteRangeAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    Task<List<TEntity>> GetAsync(CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    
    Task<TEntity?> GetFirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task SaveAsync(CancellationToken cancellationToken = default);
    
    void Update(TEntity entity);
     
    void UpdateRange(IEnumerable<TEntity> entities);
}