using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Communities;

namespace BuildingHub.Modules.Community.Application.Communities.GetList;

public sealed class GetCommunitiesQueryHandler
{
    private readonly ICommunityRepository _repository;

    public GetCommunitiesQueryHandler(
        ICommunityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetCommunityResponse>> Handle(
        GetCommunitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        var communities = await _repository.GetAllAsync(
            cancellationToken);

        return communities
            .Select(x => new GetCommunityResponse(
                x.Id,
                x.Name,
                x.Description))
            .ToList();
    }
}