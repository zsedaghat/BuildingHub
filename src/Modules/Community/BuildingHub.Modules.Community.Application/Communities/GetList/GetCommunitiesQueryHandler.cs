using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Communities;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Communities.GetList;

public sealed class GetCommunitiesQueryHandler
{
    private readonly ICommunityRepository _repository;

    public GetCommunitiesQueryHandler(ICommunityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetCommunityResponse>> HandleAll(
        CancellationToken cancellationToken = default)
    {
        var communities = await _repository.GetAllAsync(cancellationToken);
        return communities
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new GetCommunityResponse(x.Id, x.Name, x.Description))
            .ToList();
    }

    public async Task<PagedResult<GetCommunityResponse>> Handle(
        GetCommunitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = PaginationDefaults.Normalize(
            query.PageNumber, query.PageSize);

        var result = await _repository.GetPagedAsync(
            pageNumber, pageSize, cancellationToken);

        var items = result.Items.Select(x => new GetCommunityResponse(
            x.Id, x.Name, x.Description)).ToList();

        return new PagedResult<GetCommunityResponse>(
            items, result.PageNumber, result.PageSize, result.TotalCount);
    }
}
