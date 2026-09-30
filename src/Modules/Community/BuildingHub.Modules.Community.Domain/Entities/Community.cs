using BuildingHub.Modules.Community.Domain.Events;
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

/// <summary>
/// Represents a community within BuildingHub.
/// A community is the aggregate root for managing community-level information
/// such as its name and description.
/// 
/// var community = Community.Create(
///   "برج آفتاب",
///  "مجتمع ۵ طبقه با ۲۰ واحد"
///);
/// </summary>
public class Community : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private Community()
    {
    }

    private Community(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public static Community Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Community name is required.");

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
            throw new ArgumentException("Community name is required.");

        Name = name;
        Description = description;
    }
}