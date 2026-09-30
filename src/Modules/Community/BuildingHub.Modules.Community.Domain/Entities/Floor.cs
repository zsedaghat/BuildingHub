using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

public class Floor : Entity
{
    public Guid BuildingId { get; private set; }

    public string Name { get; private set; } = null!;

    public int FloorNumber { get; private set; }

    public int DisplayOrder { get; private set; }

    private Floor()
    {
    }

    private Floor(
        Guid buildingId,
        string name,
        int floorNumber,
        int displayOrder)
    {
        BuildingId = buildingId;
        Name = name;
        FloorNumber = floorNumber;
        DisplayOrder = displayOrder;
    }

    public static Floor Create(
        Guid buildingId,
        string name,
        int floorNumber,
        int displayOrder = 0)
    {
        if (buildingId == Guid.Empty)
            throw new ArgumentException("BuildingId is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Floor name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("DisplayOrder cannot be negative.");

        return new Floor(
            buildingId,
            name,
            floorNumber,
            displayOrder);
    }

    public void Update(
        string name,
        int floorNumber,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Floor name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("DisplayOrder cannot be negative.");

        Name = name;
        FloorNumber = floorNumber;
        DisplayOrder = displayOrder;
    }
}