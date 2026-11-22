
using BuildingHub.Modules.Community.Domain.Events;
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

public class Community : AggregateRoot
{
    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    private Community()
    {
    }

    private Community(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
    }

    public static Community Create(
        string name,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Community name is required.",
                nameof(name));

        var community = new Community(name, description);

        community.AddDomainEvent(
            new CommunityCreatedEvent(
                community.Id,
                DateTime.UtcNow));

        return community;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Community name is required.",
                nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }
}
