using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using Absence.Infrastructure.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Repositories;

internal class AbsenceEventRepository : IAbsenceEventRepository
{
    private bool _isDisposed = false;
    protected readonly AbsenceContext _context;
    private readonly IRepository<AbsenceRequestEntity> _repository;
    protected readonly DbSet<AbsenceRequestEntity> _entities;

    public AbsenceEventRepository(AbsenceContext context, IRepository<AbsenceRequestEntity> repository)
    {
        _context = context;
        _entities = context.Set<AbsenceRequestEntity>();
        _repository = repository;
    }

    ~AbsenceEventRepository()
    {
        DisposeAsync(false).GetAwaiter().GetResult();
    }

    public Task<List<AbsenceRequestEntity>> GetAsync(Func<IQueryable<AbsenceRequestEntity>, IQueryable<AbsenceRequestEntity>>[]? queries = null!, CancellationToken cancellationToken = default)
    {
        IQueryable<AbsenceRequestEntity> query = _entities;

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

    public void Delete(AbsenceRequestEntity entity) =>
        _repository.Delete(entity);

    public void DeleteRange(IEnumerable<AbsenceRequestEntity> entities) =>
        _repository.DeleteRange(entities);

    public Task<AbsenceRequestEntity?> GetFirstOrDefaultAsync(Func<IQueryable<AbsenceRequestEntity>, IQueryable<AbsenceRequestEntity>>[]? queries = null, CancellationToken cancellationToken = default) =>
        _repository.GetFirstOrDefaultAsync(queries, cancellationToken);

    public Task<AbsenceRequestEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task InsertAsync(AbsenceRequestEntity entity, CancellationToken cancellationToken = default) =>
        _repository.InsertAsync(entity, cancellationToken);

    public Task InsertRangeAsync(IEnumerable<AbsenceRequestEntity> entities, CancellationToken cancellationToken = default) =>
        _repository.InsertRangeAsync(entities, cancellationToken);

    public void Update(AbsenceRequestEntity entity) =>
        _repository.Update(entity);

    public void UpdateRange(IEnumerable<AbsenceRequestEntity> entities) =>
        _repository.UpdateRange(entities);

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _repository.SaveAsync(cancellationToken);
}