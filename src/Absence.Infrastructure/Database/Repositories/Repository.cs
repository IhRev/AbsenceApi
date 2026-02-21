using Absence.Domain.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Repositories;

internal class Repository<TEntity>(
    AbsenceContext context, 
    ISpecificationEvaluator specificationEvaluator
) : IRepository<TEntity>
    where TEntity : class
{
    protected readonly AbsenceContext _context = context;
    protected readonly DbSet<TEntity> _entities = context.Set<TEntity>();
    protected readonly ISpecificationEvaluator _specificationEvaluator = specificationEvaluator;

    public Repository(AbsenceContext context)
        : this(context, SpecificationEvaluator.Default)
    {
    }

    public virtual Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).AnyAsync(cancellationToken);

    public virtual Task<List<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).ToListAsync(cancellationToken);

    public virtual Task<TEntity?> GetFirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public virtual Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => 
        _entities.FindAsync([id], cancellationToken).AsTask();

    public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _entities.AddAsync(entity, cancellationToken).AsTask();

    public virtual Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        _entities.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(TEntity entity) => 
        _context.Entry(entity).State = EntityState.Modified;

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Update(entity);
        }
    }

    public virtual void Delete(TEntity entity) => 
        _entities.Remove(entity);

    public virtual void DeleteRange(IEnumerable<TEntity> entities) => 
        _entities.RemoveRange(entities);

    public virtual void DeleteRangeAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        DeleteRange(query);
    }

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false) =>
        _specificationEvaluator.GetQuery(_entities, specification, evaluateCriteriaOnly);
}