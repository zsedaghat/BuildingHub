using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class PersonRepository
    : Repository<Person>, IPersonRepository
{
    public PersonRepository(
        CommunityDbContext context)
        : base(context)
    {
    }

    public async Task<Person?> GetByMobileAsync(
        string mobile,
        CancellationToken cancellationToken = default)
    {
        return await Context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Mobile == mobile,
                cancellationToken);
    }
}