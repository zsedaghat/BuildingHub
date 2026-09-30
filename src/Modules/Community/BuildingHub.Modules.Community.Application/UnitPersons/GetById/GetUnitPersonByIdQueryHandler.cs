using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetById;

public sealed class GetUnitPersonByIdQueryHandler
{
    private readonly IUnitPersonRepository _repository;

    public GetUnitPersonByIdQueryHandler(
        IUnitPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUnitPersonByIdResponse?> Handle(
        GetUnitPersonByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var relation = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (relation is null)
            return null;

        return new GetUnitPersonByIdResponse(
            relation.Id,
            relation.UnitId,
            relation.PersonId,
            relation.Role,
            relation.IsPrimary,
            relation.StartDate,
            relation.EndDate);
    }
}