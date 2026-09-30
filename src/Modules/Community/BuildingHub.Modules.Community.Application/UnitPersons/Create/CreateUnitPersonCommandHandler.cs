using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.Create;

public sealed class CreateUnitPersonCommandHandler
{
    private readonly IUnitPersonRepository _repository;

    public CreateUnitPersonCommandHandler(
        IUnitPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateUnitPersonResponse> Handle(
        CreateUnitPersonCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingRelations = await _repository.GetByUnitIdAsync(
            command.UnitId,
            cancellationToken);

        var alreadyExists = existingRelations.Any(x =>
            x.PersonId == command.PersonId &&
            x.Role == command.Role);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "This person already has this role for this unit.");
        }

        var unitPerson = UnitPerson.Create(
            command.UnitId,
            command.PersonId,
            command.Role,
            command.IsPrimary,
            command.StartDate,
            command.EndDate);

        await _repository.AddAsync(
            unitPerson,
            cancellationToken);

        return new CreateUnitPersonResponse(
            unitPerson.Id,
            unitPerson.UnitId,
            unitPerson.PersonId,
            unitPerson.Role,
            unitPerson.IsPrimary,
            unitPerson.StartDate,
            unitPerson.EndDate);
    }
}