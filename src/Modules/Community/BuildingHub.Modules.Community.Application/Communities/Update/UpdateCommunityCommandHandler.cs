using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Communities;

namespace BuildingHub.Modules.Community.Application.Communities.Update;

public sealed class UpdateCommunityCommandHandler
{
    private readonly ICommunityRepository _repository;

    public UpdateCommunityCommandHandler(
        ICommunityRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateCommunityResponse?> Handle(
        UpdateCommunityCommand command,
        CancellationToken cancellationToken = default)
    {
        var community = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (community is null)
            return null;

        community.Update(
            command.Name,
            command.Description);

        await _repository.UpdateAsync(
            community,
            cancellationToken);

        return new UpdateCommunityResponse(
            community.Id,
            community.Name,
            community.Description);
    }
}