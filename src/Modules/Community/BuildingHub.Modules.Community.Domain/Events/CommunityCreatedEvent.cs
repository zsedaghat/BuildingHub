using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Events;

public sealed record CommunityCreatedEvent(
    Guid CommunityId,
    DateTime OccurredOn) : IDomainEvent;