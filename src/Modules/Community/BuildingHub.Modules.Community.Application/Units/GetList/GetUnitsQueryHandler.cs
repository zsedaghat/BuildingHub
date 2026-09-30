using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Units.GetList;

public sealed class GetUnitsQueryHandler
{
    private readonly IUnitRepository _repository;

    public GetUnitsQueryHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetUnitsResponse>> Handle(
        GetUnitsQuery query,
        CancellationToken cancellationToken = default)
    {
        var units = await _repository.GetAllAsync(cancellationToken);

        return units
            .Select(x => new GetUnitsResponse(
                x.Id,
                x.BuildingId,
                x.FloorId,
                x.UnitNumber,
                x.Type,
                x.Area,
                x.IsActive))
            .ToList();
    }
}