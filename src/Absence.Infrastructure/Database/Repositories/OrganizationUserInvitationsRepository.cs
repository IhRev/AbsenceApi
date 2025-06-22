using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Repositories;

internal class OrganizationUserInvitationsRepository : IOrganizationUserInvitationsRepository
{
    private bool _isDisposed = false;
    protected readonly AbsenceContext _context;
    private readonly IRepository<OrganizationUserInvitationEntity> _repository;
    protected readonly DbSet<OrganizationUserInvitationEntity> _entities;

    public OrganizationUserInvitationsRepository(AbsenceContext context, IRepository<OrganizationUserInvitationEntity> repository)
    {
        _context = context;
        _repository = repository;
        _entities = context.Set<OrganizationUserInvitationEntity>();
    }

    ~OrganizationUserInvitationsRepository()
    {
        DisposeAsync(false).GetAwaiter().GetResult();
    }

    public Task<List<OrganizationUserInvitationEntity>> GetAsync(Func<IQueryable<OrganizationUserInvitationEntity>, IQueryable<OrganizationUserInvitationEntity>>[] queries = null!, CancellationToken cancellationToken = default)
    {
        IQueryable<OrganizationUserInvitationEntity> query = _entities;

        if (queries != null)
        {
            query = queries
                .Aggregate(query, (current, next) => next(current))
                .Include(_ => _.Organization)
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

    public void Delete(OrganizationUserInvitationEntity entity) =>
        _repository.Delete(entity);

    public void DeleteRange(IEnumerable<OrganizationUserInvitationEntity> entities) =>
        _repository.DeleteRange(entities);

    public Task<OrganizationUserInvitationEntity?> GetFirstOrDefaultAsync(Func<IQueryable<OrganizationUserInvitationEntity>, IQueryable<OrganizationUserInvitationEntity>>[] queries = null!, CancellationToken cancellationToken = default) =>
        _repository.GetFirstOrDefaultAsync(queries, cancellationToken);

    public Task<OrganizationUserInvitationEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task InsertAsync(OrganizationUserInvitationEntity entity, CancellationToken cancellationToken = default) =>
        _repository.InsertAsync(entity, cancellationToken);

    public Task InsertRangeAsync(IEnumerable<OrganizationUserInvitationEntity> entities, CancellationToken cancellationToken = default) =>
        _repository.InsertRangeAsync(entities, cancellationToken);

    public void Update(OrganizationUserInvitationEntity entity) =>
        _repository.Update(entity);

    public void UpdateRange(IEnumerable<OrganizationUserInvitationEntity> entities) =>
        _repository.UpdateRange(entities);

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _repository.SaveAsync(cancellationToken);
}