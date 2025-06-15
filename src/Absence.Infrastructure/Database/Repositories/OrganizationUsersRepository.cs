using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Repositories;

internal class OrganizationUsersRepository : IOrganizationUsersRepository
{
    private bool _isDisposed = false;
    protected readonly AbsenceContext _context;
    private readonly IRepository<OrganizationUserEntity> _repository;
    protected readonly DbSet<OrganizationUserEntity> _entities;

    public OrganizationUsersRepository(AbsenceContext context, IRepository<OrganizationUserEntity> repository)
    {
        _context = context;
        _repository = repository;
        _entities = context.Set<OrganizationUserEntity>();
    }

    ~OrganizationUsersRepository()
    {
        DisposeAsync(false).GetAwaiter().GetResult();
    }

    public Task<List<OrganizationUserEntity>> GetAsync(Func<IQueryable<OrganizationUserEntity>, IQueryable<OrganizationUserEntity>>[] queries = null!, CancellationToken cancellationToken = default)
    {
        IQueryable<OrganizationUserEntity> query = _entities;

        if (queries != null)
        {
            query = queries
                .Aggregate(query, (current, next) => next(current))
                .Include(_ => _.Organization);
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

    public void Delete(OrganizationUserEntity entity) =>
        _repository.Delete(entity);

    public void DeleteRange(IEnumerable<OrganizationUserEntity> entities) =>
        _repository.DeleteRange(entities);

    public Task<OrganizationUserEntity?> GetFirstOrDefaultAsync(Func<IQueryable<OrganizationUserEntity>, IQueryable<OrganizationUserEntity>>[] queries = null!, CancellationToken cancellationToken = default) =>
        _repository.GetFirstOrDefaultAsync(queries, cancellationToken);

    public Task<OrganizationUserEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task InsertAsync(OrganizationUserEntity entity, CancellationToken cancellationToken = default) =>
        _repository.InsertAsync(entity, cancellationToken);

    public Task InsertRangeAsync(IEnumerable<OrganizationUserEntity> entities, CancellationToken cancellationToken = default) =>
        _repository.InsertRangeAsync(entities, cancellationToken);

    public void Update(OrganizationUserEntity entity) =>
        _repository.Update(entity);

    public void UpdateRange(IEnumerable<OrganizationUserEntity> entities) =>
        _repository.UpdateRange(entities);

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _repository.SaveAsync(cancellationToken);
}