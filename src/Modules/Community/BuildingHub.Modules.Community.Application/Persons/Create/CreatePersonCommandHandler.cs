using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.ValueObjects;
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
        var normalizedMobile =
    MobileNumberNormalizer.Normalize(command.Mobile);

        var existingPerson = await _repository.GetByMobileAsync(
            normalizedMobile,
            cancellationToken);

        if (existingPerson is not null)
            throw new InvalidOperationException(
                "A person with this mobile number already exists.");

        var person = PersonEntity.Create(
            command.FirstName,
            command.LastName,
            normalizedMobile,
            command.Email,
            command.NationalId);

        await _repository.AddAsync(
            person,
            cancellationToken);

        return new CreatePersonResponse(
            person.Id,
            person.FirstName,
            person.LastName,
           normalizedMobile,
            person.Email,
            person.NationalId);
    }
}