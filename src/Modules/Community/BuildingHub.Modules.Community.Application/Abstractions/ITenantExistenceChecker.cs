namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface ITenantExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}