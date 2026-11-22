
using BuildingHub.Modules.Tenancy.Domain.Entities;

namespace BuildingHub.Modules.Tenancy.Application.Abstractions;

public interface ITenantCommunityAssignmentRepository
{
    Task<TenantCommunityAssignment?> GetEffectiveByCommunityAsync(
        Guid communityId,
        DateTime atUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TenantCommunityAssignment>>
        GetEffectiveByTenantAsync(
            Guid tenantId,
            DateTime atUtc,
            CancellationToken cancellationToken = default);

    Task<bool> HasEffectiveAssignmentAsync(
        Guid tenantId,
        Guid communityId,
        DateTime atUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TenantCommunityAssignment assignment,
        CancellationToken cancellationToken = default);
}
