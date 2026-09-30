using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class UnitPersonRepository
    : Repository<UnitPerson>, IUnitPersonRepository
{
    public UnitPersonRepository(CommunityDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<UnitPerson>> GetByUnitIdAsync(
        Guid unitId,
        CancellationToken cancellationToken = default)
    {
        return await Context.UnitPersons
            .AsNoTracking()
            .Where(x => x.UnitId == unitId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnitPerson>> GetByPersonIdAsync(
        Guid personId,
        CancellationToken cancellationToken = default)
    {
        return await Context.UnitPersons
            .AsNoTracking()
            .Where(x => x.PersonId == personId)
            .ToListAsync(cancellationToken);
    }
}