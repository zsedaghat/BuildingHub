using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public class Repository<T> : IRepository<T>
    where T : class
{
    protected readonly CommunityDbContext Context;

    public Repository(CommunityDbContext context)
    {
        Context = context;
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(
            entity,
            cancellationToken);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>()
            .FindAsync(
                new object[] { id },
                cancellationToken);
    }

    public async Task UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Update(entity);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Remove(entity);

        await Context.SaveChangesAsync(cancellationToken);
    }
}