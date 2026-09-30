using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetByPersonId;

public sealed class GetUnitPersonsByPersonIdQueryHandler
{
    private readonly IUnitPersonRepository _unitPersonRepository;
    private readonly IUnitRepository _unitRepository;

    public GetUnitPersonsByPersonIdQueryHandler(
        IUnitPersonRepository unitPersonRepository,
        IUnitRepository unitRepository)
    {
        _unitPersonRepository = unitPersonRepository;
        _unitRepository = unitRepository;
    }

    public async Task<IReadOnlyList<GetUnitPersonsByPersonIdResponse>> Handle(
        GetUnitPersonsByPersonIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var relations = await _unitPersonRepository.GetByPersonIdAsync(
            query.PersonId,
            cancellationToken);

        var result = new List<GetUnitPersonsByPersonIdResponse>();

        foreach (var relation in relations)
        {
            var unit = await _unitRepository.GetByIdAsync(
                relation.UnitId,
                cancellationToken);

            if (unit is null)
                continue;

            result.Add(new GetUnitPersonsByPersonIdResponse(
                relation.Id,
                relation.UnitId,
                relation.PersonId,
                unit.UnitNumber,
                relation.Role,
                relation.IsPrimary,
                relation.StartDate,
                relation.EndDate));
        }

        return result;
    }
}