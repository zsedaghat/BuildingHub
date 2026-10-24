
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Units.GetActiveByBuildingId;
using BuildingHub.Modules.Community.Domain.Entities;
using MediatR;

namespace BuildingHub.Api.Services;

public sealed class BuildingUnitProvider : IBuildingUnitProvider
{
    private readonly ISender _sender;
    private readonly IUnitPersonRepository _unitPersonRepository;

    public BuildingUnitProvider(
        ISender sender,
        IUnitPersonRepository unitPersonRepository)
    {
        _sender = sender;
        _unitPersonRepository = unitPersonRepository;
    }

    public async Task<IReadOnlyList<BuildingUnitInfo>> GetActiveUnitsAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        var units = await _sender.Send(
            new GetActiveUnitsByBuildingIdQuery(buildingId),
            cancellationToken);

        var result = new List<BuildingUnitInfo>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var unit in units)
        {
            var relationships =
                await _unitPersonRepository.GetByUnitIdAsync(
                    unit.UnitId,
                    cancellationToken);

            var residentCount = relationships.Count(person =>
                (person.Role == UnitPersonRole.Resident ||
                 person.Role == UnitPersonRole.Tenant) &&
                (!person.StartDate.HasValue ||
                 person.StartDate.Value <= today) &&
                (!person.EndDate.HasValue ||
                 person.EndDate.Value >= today));

            result.Add(new BuildingUnitInfo(
                unit.UnitId,
                unit.UnitNumber,
                unit.Area,
                residentCount));
        }

        return result;
    }
}
