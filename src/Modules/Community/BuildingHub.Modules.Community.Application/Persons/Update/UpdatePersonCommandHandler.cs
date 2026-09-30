using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Persons.Update;

public sealed class UpdatePersonCommandHandler
{
    private readonly IPersonRepository _repository;

    public UpdatePersonCommandHandler(
        IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdatePersonResponse?> Handle(
        UpdatePersonCommand command,
        CancellationToken cancellationToken = default)
    {
        var person = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (person is null)
            return null;

        var existingPerson = await _repository.GetByMobileAsync(
            command.Mobile,
            cancellationToken);

        if (existingPerson is not null &&
            existingPerson.Id != command.Id)
        {
            throw new InvalidOperationException(
                "A person with this mobile number already exists.");
        }

        person.Update(
            command.FirstName,
            command.LastName,
            command.Mobile,
            command.Email,
            command.NationalId);

        await _repository.UpdateAsync(
            person,
            cancellationToken);

        return new UpdatePersonResponse(
            person.Id,
            person.FirstName,
            person.LastName,
            person.Mobile,
            person.Email,
            person.NationalId);
    }
}