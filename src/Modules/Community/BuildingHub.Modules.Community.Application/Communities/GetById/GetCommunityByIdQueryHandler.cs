using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Communities;

namespace BuildingHub.Modules.Community.Application.Communities.GetById;

public sealed class GetCommunityByIdQueryHandler
{
    private readonly ICommunityRepository _repository;

    public GetCommunityByIdQueryHandler(
        ICommunityRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCommunityByIdResponse?> Handle(
        GetCommunityByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var community = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (community is null)
            return null;

        return new GetCommunityByIdResponse(
            community.Id,
            community.Name,
            community.Description);
    }
}