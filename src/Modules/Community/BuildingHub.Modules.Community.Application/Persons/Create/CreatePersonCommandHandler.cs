using BuildingHub.Modules.Community.Application.Abstractions;

using PersonEntity =
    BuildingHub.Modules.Community.Domain.Entities.Person;

namespace BuildingHub.Modules.Community.Application.Persons.Create;

public sealed class CreatePersonCommandHandler
{
    private readonly IPersonRepository _repository;

    public CreatePersonCommandHandler(
        IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreatePersonResponse> Handle(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingPerson = await _repository.GetByMobileAsync(
            command.Mobile,
            cancellationToken);

        if (existingPerson is not null)
            throw new InvalidOperationException(
                "A person with this mobile number already exists.");

        var person = PersonEntity.Create(
            command.FirstName,
            command.LastName,
            command.Mobile,
            command.Email,
            command.NationalId);

        await _repository.AddAsync(
            person,
            cancellationToken);

        return new CreatePersonResponse(
            person.Id,
            person.FirstName,
            person.LastName,
            person.Mobile,
            person.Email,
            person.NationalId);
    }
}