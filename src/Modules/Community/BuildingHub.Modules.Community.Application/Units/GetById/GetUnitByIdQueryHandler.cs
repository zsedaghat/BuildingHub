using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Units.GetById;

public sealed class GetUnitByIdQueryHandler
{
    private readonly IUnitRepository _repository;

    public GetUnitByIdQueryHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUnitByIdResponse?> Handle(
        GetUnitByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var unit = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (unit is null)
            return null;

        return new GetUnitByIdResponse(
            unit.Id,
            unit.BuildingId,
            unit.FloorId,
            unit.UnitNumber,
            unit.Type,
            unit.Area,
            unit.IsActive);
    }
}