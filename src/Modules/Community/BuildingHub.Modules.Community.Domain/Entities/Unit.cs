using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

public enum UnitType
{
    Apartment = 1,
    Parking = 2,
    Storage = 3,
    Commercial = 4,
    Other = 5
}

public class Unit : Entity
{
    public Guid BuildingId { get; private set; }

    public Guid? FloorId { get; private set; }

    public string UnitNumber { get; private set; } = null!;

    public UnitType Type { get; private set; }

    public decimal? Area { get; private set; }

    public bool IsActive { get; private set; }

    private Unit()
    {
    }

    private Unit(
        Guid buildingId,
        Guid? floorId,
        string unitNumber,
        UnitType type,
        decimal? area)
    {
        BuildingId = buildingId;
        FloorId = floorId;
        UnitNumber = unitNumber;
        Type = type;
        Area = area;
        IsActive = true;
    }

    public static Unit Create(
        Guid buildingId,
        Guid? floorId,
        string unitNumber,
        UnitType type,
        decimal? area = null)
    {
        if (buildingId == Guid.Empty)
            throw new ArgumentException("BuildingId is required.");

        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("Unit number is required.");

        if (area.HasValue && area <= 0)
            throw new ArgumentException("Area must be greater than zero.");

        return new Unit(
            buildingId,
            floorId,
            unitNumber,
            type,
            area);
    }

    public void Update(
        string unitNumber,
        UnitType type,
        decimal? area)
    {
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("Unit number is required.");

        if (area.HasValue && area <= 0)
            throw new ArgumentException("Area must be greater than zero.");

        UnitNumber = unitNumber;
        Type = type;
        Area = area;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}