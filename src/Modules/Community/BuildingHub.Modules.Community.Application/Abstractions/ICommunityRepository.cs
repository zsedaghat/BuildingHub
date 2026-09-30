using CommunityEntity =
    BuildingHub.Modules.Community.Domain.Entities.Community;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface ICommunityRepository : IRepository<CommunityEntity>
{
}