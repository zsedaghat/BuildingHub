using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Units.GetList;

public sealed class GetUnitsQueryHandler
{
    private readonly IUnitRepository _repository;

    public GetUnitsQueryHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetUnitsResponse>> HandleAll(
        CancellationToken cancellationToken = default)
    {
        var units = await _repository.GetAllAsync(cancellationToken);
        return units
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Select(x => new GetUnitsResponse(
                x.Id,
                x.BuildingId,
                x.FloorId,
                x.UnitNumber,
                x.Type,
                x.Area,
                x.IsActive))
            .ToList();
    }

    public async Task<PagedResult<GetUnitsResponse>> Handle(
        GetUnitsQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = PaginationDefaults.Normalize(
            query.PageNumber, query.PageSize);

        var result = await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            query.BuildingId,
            query.FloorId,
            cancellationToken);

        var items = result.Items.Select(x => new GetUnitsResponse(
            x.Id,
            x.BuildingId,
            x.FloorId,
            x.UnitNumber,
            x.Type,
            x.Area,
            x.IsActive)).ToList();

        return new PagedResult<GetUnitsResponse>(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }
}
