using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using CommunityEntity =
    BuildingHub.Modules.Community.Domain.Entities.Community;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class CommunityRepository
    : Repository<CommunityEntity>, ICommunityRepository
{
    public CommunityRepository(
        CommunityDbContext context)
        : base(context)
    {
    }
}