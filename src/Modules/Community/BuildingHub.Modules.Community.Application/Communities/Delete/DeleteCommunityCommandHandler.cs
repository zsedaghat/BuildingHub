using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Communities;

namespace BuildingHub.Modules.Community.Application.Communities.Delete;

public sealed class DeleteCommunityCommandHandler
{
    private readonly ICommunityRepository _repository;

    public DeleteCommunityCommandHandler(
        ICommunityRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteCommunityCommand command,
        CancellationToken cancellationToken = default)
    {
        var community = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (community is null)
            return false;

        await _repository.DeleteAsync(
            community,
            cancellationToken);

        return true;
    }
}