using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Buildings.GetList;

public sealed class GetBuildingsQueryHandler
{
    private readonly IBuildingRepository _repository;

    public GetBuildingsQueryHandler(IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetBuildingResponse>> HandleAll(
        CancellationToken cancellationToken = default)
    {
        var buildings = await _repository.GetAllAsync(cancellationToken);
        return buildings
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new GetBuildingResponse(
                x.Id, x.CommunityId, x.Name, x.Description, x.DisplayOrder))
            .ToList();
    }

    public async Task<PagedResult<GetBuildingResponse>> Handle(
        GetBuildingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = PaginationDefaults.Normalize(
            query.PageNumber, query.PageSize);

        var result = await _repository.GetPagedAsync(
            pageNumber, pageSize, cancellationToken);

        var items = result.Items.Select(x => new GetBuildingResponse(
            x.Id, x.CommunityId, x.Name, x.Description, x.DisplayOrder)).ToList();

        return new PagedResult<GetBuildingResponse>(
            items, result.PageNumber, result.PageSize, result.TotalCount);
    }
}
