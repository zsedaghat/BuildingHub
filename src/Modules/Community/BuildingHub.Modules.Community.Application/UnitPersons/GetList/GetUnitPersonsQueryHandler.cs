using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetList;

public sealed class GetUnitPersonsQueryHandler
{
    private readonly IUnitPersonRepository _repository;

    public GetUnitPersonsQueryHandler(
        IUnitPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetUnitPersonsResponse>> Handle(
        GetUnitPersonsQuery query,
        CancellationToken cancellationToken = default)
    {
        var relations = await _repository.GetAllAsync(
            cancellationToken);

        return relations
            .Select(x => new GetUnitPersonsResponse(
                x.Id,
                x.UnitId,
                x.PersonId,
                x.Role,
                x.IsPrimary,
                x.StartDate,
                x.EndDate))
            .ToList();
    }
}