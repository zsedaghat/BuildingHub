using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Units.Delete;

public sealed class DeleteUnitCommandHandler
{
    private readonly IUnitRepository _repository;

    public DeleteUnitCommandHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (unit is null)
            return false;

        await _repository.DeleteAsync(
            unit,
            cancellationToken);

        return true;
    }
}