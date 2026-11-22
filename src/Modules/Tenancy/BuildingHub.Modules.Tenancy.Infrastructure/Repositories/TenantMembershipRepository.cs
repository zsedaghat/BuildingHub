
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Entities;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Repositories;

public sealed class TenantMembershipRepository
    : ITenantMembershipRepository
{
    private readonly TenancyDbContext _context;

    public TenantMembershipRepository(TenancyDbContext context)
    {
        _context = context;
    }

    public async Task<TenantUserMembership?> GetByIdAsync(
        Guid membershipId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantUserMemberships
            .FirstOrDefaultAsync(
                x => x.Id == membershipId,
                cancellationToken);
    }

    public async Task<bool> HasAnotherActiveAdminAsync(
        Guid tenantId,
        Guid excludedMembershipId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantUserMemberships
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.Id != excludedMembershipId &&
                    x.Role == TenantMembershipRole.TenantAdmin &&
                    x.Status == TenantMembershipStatus.Active &&
                    x.StartAtUtc <= nowUtc &&
                    (x.EndAtUtc == null ||
                     x.EndAtUtc > nowUtc),
                cancellationToken);
    }

    public async Task<bool> HasOpenMembershipAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantUserMemberships
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.UserId == userId &&
                    x.EndAtUtc == null,
                cancellationToken);
    }

    public async Task AddAsync(
        TenantUserMembership membership,
        CancellationToken cancellationToken = default)
    {
        await _context.TenantUserMemberships
            .AddAsync(membership, cancellationToken);
    }

    public async Task<bool> HasAnyOpenMembershipAsync(
    Guid tenantId,
    CancellationToken cancellationToken = default)
    {
        return await _context.TenantUserMemberships
            .AsNoTracking()
            .AnyAsync(
                x => x.TenantId == tenantId &&
                     x.EndAtUtc == null,
                cancellationToken);
    }
}
