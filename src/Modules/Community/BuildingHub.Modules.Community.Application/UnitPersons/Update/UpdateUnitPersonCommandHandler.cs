using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.UnitPersons.Update;

public sealed class UpdateUnitPersonCommandHandler
{
    private readonly IUnitPersonRepository _repository;

    public UpdateUnitPersonCommandHandler(
        IUnitPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateUnitPersonResponse?> Handle(
        UpdateUnitPersonCommand command,
        CancellationToken cancellationToken = default)
    {
        var relation = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (relation is null)
            return null;

        var existingRelations = await _repository.GetByUnitIdAsync(
            relation.UnitId,
            cancellationToken);

        var duplicate = existingRelations.Any(x =>
            x.Id != command.Id &&
            x.PersonId == relation.PersonId &&
            x.Role == command.Role);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "This person already has this role for this unit.");
        }

        relation.Update(
            command.Role,
            command.IsPrimary,
            command.StartDate,
            command.EndDate);

        await _repository.UpdateAsync(
            relation,
            cancellationToken);

        return new UpdateUnitPersonResponse(
            relation.Id,
            relation.UnitId,
            relation.PersonId,
            relation.Role,
            relation.IsPrimary,
            relation.StartDate,
            relation.EndDate);
    }
}