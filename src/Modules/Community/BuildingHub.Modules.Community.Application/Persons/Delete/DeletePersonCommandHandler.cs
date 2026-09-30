using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Persons.Delete;

public sealed class DeletePersonCommandHandler
{
    private readonly IPersonRepository _repository;

    public DeletePersonCommandHandler(
        IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeletePersonCommand command,
        CancellationToken cancellationToken = default)
    {
        var person = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (person is null)
            return false;

        await _repository.DeleteAsync(
            person,
            cancellationToken);

        return true;
    }
}