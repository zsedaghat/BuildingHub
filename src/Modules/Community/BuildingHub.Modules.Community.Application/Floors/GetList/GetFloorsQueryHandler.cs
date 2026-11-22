using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Floors.GetList;

public sealed class GetFloorsQueryHandler
{
    private readonly IFloorRepository _repository;

    public GetFloorsQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GetFloorsResponse>> Handle(
        GetFloorsQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = PaginationDefaults.Normalize(
            query.PageNumber, query.PageSize);

        var result = await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            query.BuildingId,
            cancellationToken);

        var items = result.Items
            .Select(x => new GetFloorsResponse(
                x.Id,
                x.BuildingId,
                x.Name,
                x.FloorNumber,
                x.DisplayOrder))
            .ToList();

        return new PagedResult<GetFloorsResponse>(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }
}
