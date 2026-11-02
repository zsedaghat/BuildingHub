using BuildingHub.SharedKernel.Common.Pagination;
using CommunityEntity = BuildingHub.Modules.Community.Domain.Entities.Community;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface ICommunityRepository : IRepository<CommunityEntity>
{
    Task<PagedResult<CommunityEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
