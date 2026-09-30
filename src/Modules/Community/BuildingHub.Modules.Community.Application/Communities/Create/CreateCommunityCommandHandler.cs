using BuildingHub.Modules.Community.Application.Abstractions;
using CommunityEntity =
    BuildingHub.Modules.Community.Domain.Entities.Community;

namespace BuildingHub.Modules.Community.Application.Communities.Create;

public sealed class CreateCommunityCommandHandler
{
    private readonly ICommunityRepository _repository;

    public CreateCommunityCommandHandler(ICommunityRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateCommunityResponse> Handle(
        CreateCommunityCommand command,
        CancellationToken cancellationToken = default)
    {
        var community = CommunityEntity.Create(
            command.Name,
            command.Description);

        await _repository.AddAsync(
            community,
            cancellationToken);

        return new CreateCommunityResponse(
            community.Id,
            community.Name,
            community.Description);
    }
}