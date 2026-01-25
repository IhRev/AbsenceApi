using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using Absence.Infrastructure.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Repositories;

internal class AbsenceEventRepository : IAbsenceEventRepository
{
    private bool _isDisposed = false;
    protected readonly AbsenceContext _context;
    private readonly IRepository<AbsenceEventEntity> _repository;
    protected readonly DbSet<AbsenceEventEntity> _entities;

    public AbsenceEventRepository(AbsenceContext context, IRepository<AbsenceEventEntity> repository)
    {
        _context = context;
        _entities = context.Set<AbsenceEventEntity>();
        _repository = repository;
    }

    ~AbsenceEventRepository()
    {
        DisposeAsync(false).GetAwaiter().GetResult();
    }

    public Task<List<AbsenceEventEntity>> GetAsync(Func<IQueryable<AbsenceEventEntity>, IQueryable<AbsenceEventEntity>>[]? queries = null!, CancellationToken cancellationToken = default)
    {
        IQueryable<AbsenceEventEntity> query = _entities;

        if (queries != null)
        {
            query = queries
                .Aggregate(query, (current, next) => next(current))
                .Include(_ => _.User);
        }

        return query.ToListAsync(cancellationToken);
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

    public void Delete(AbsenceEventEntity entity) =>
        _repository.Delete(entity);

    public void DeleteRange(IEnumerable<AbsenceEventEntity> entities) =>
        _repository.DeleteRange(entities);

    public Task<AbsenceEventEntity?> GetFirstOrDefaultAsync(Func<IQueryable<AbsenceEventEntity>, IQueryable<AbsenceEventEntity>>[]? queries = null, CancellationToken cancellationToken = default) =>
        _repository.GetFirstOrDefaultAsync(queries, cancellationToken);

    public Task<AbsenceEventEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task InsertAsync(AbsenceEventEntity entity, CancellationToken cancellationToken = default) =>
        _repository.InsertAsync(entity, cancellationToken);

    public Task InsertRangeAsync(IEnumerable<AbsenceEventEntity> entities, CancellationToken cancellationToken = default) =>
        _repository.InsertRangeAsync(entities, cancellationToken);

    public void Update(AbsenceEventEntity entity) =>
        _repository.Update(entity);

    public void UpdateRange(IEnumerable<AbsenceEventEntity> entities) =>
        _repository.UpdateRange(entities);

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _repository.SaveAsync(cancellationToken);
}