using Absence.Domain.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Repositories;

internal class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private bool _isDisposed = false;
    protected readonly AbsenceContext _context;
    protected readonly DbSet<TEntity> _entities;

    public Repository(AbsenceContext context)
    {
        _context = context;
        _entities = context.Set<TEntity>();
    }

    ~Repository()
    {
        DisposeAsync(false).GetAwaiter().GetResult();
    }

    public virtual Task<List<TEntity>> GetAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>>[] queries = null!, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _entities;

        if (queries != null)
        {
            query = queries.Aggregate(query, (current, next) => next(current));
        }

        return query.ToListAsync(cancellationToken);
    } 

    public virtual Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => 
        _entities.FindAsync([id], cancellationToken).AsTask();

    public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _entities.AddAsync(entity, cancellationToken).AsTask();

    public virtual Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        _entities.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(TEntity entity)
    {
        _entities.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Update(entity);
        }
    }

    public virtual async Task DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        TEntity? entityToDelete = await GetByIdAsync(id, cancellationToken);
        if (entityToDelete is null)
        {
            return;
        }

        Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entity)
    {
        AttachIfDetached(entity);

        _entities.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            AttachIfDetached(entity);
        }

        _entities.RemoveRange(entities);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected async ValueTask DisposeAsync(bool isDisposing)
    {
        if (_isDisposed)
            return;

        if (isDisposing)
        {
            await _context.DisposeAsync();
        }

        _isDisposed = true;
    }

    private void AttachIfDetached(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _entities.Attach(entity);
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}