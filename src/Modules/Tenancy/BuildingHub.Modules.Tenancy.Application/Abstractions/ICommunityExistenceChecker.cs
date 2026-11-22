namespace BuildingHub.Modules.Tenancy.Application.Abstractions;

public interface ICommunityExistenceChecker
{
    Task<bool> HasCommunitiesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}