namespace BuildingHub.SharedKernel.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}