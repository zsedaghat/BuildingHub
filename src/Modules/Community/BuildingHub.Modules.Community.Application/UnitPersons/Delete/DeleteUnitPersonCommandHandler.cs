using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.UnitPersons.Delete;

public sealed class DeleteUnitPersonCommandHandler
{
    private readonly IUnitPersonRepository _repository;

    public DeleteUnitPersonCommandHandler(
        IUnitPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteUnitPersonCommand command,
        CancellationToken cancellationToken = default)
    {
        var relation = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (relation is null)
            return false;

        await _repository.DeleteAsync(
            relation,
            cancellationToken);

        return true;
    }
}