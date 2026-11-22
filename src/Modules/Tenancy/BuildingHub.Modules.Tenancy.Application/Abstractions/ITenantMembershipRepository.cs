
using BuildingHub.Modules.Tenancy.Domain.Entities;

namespace BuildingHub.Modules.Tenancy.Application.Abstractions;

public interface ITenantMembershipRepository
{
    Task<TenantUserMembership?> GetByIdAsync(
        Guid membershipId,
        CancellationToken cancellationToken = default);

    Task<bool> HasAnotherActiveAdminAsync(
        Guid tenantId,
        Guid excludedMembershipId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    Task<bool> HasOpenMembershipAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TenantUserMembership membership,
        CancellationToken cancellationToken = default);

    Task<bool> HasAnyOpenMembershipAsync(
    Guid tenantId,
    CancellationToken cancellationToken = default);
}
