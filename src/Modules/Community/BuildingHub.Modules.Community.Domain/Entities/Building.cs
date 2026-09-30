using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

public class Building : Entity
{
    public Guid CommunityId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public int DisplayOrder { get; private set; }

    private Building()
    {
    }

    private Building(
        Guid communityId,
        string name,
        string? description,
        int displayOrder)
    {
        CommunityId = communityId;
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
    }

    public static Building Create(
        Guid communityId,
        string name,
        string? description = null,
        int displayOrder = 0)
    {
        if (communityId == Guid.Empty)
            throw new ArgumentException("CommunityId is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Building name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("DisplayOrder cannot be negative.");

        return new Building(
            communityId,
            name,
            description,
            displayOrder);
    }

    public void Update(
        string name,
        string? description,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Building name is required.");

        if (displayOrder < 0)
            throw new ArgumentException("DisplayOrder cannot be negative.");

        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
    }
}